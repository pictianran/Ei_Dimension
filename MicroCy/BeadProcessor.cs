﻿namespace DIOS.Core
{
  internal class BeadProcessor
  {
    public NormalizationSettings Normalization { get; } = new NormalizationSettings();
    private float _greenMin;
    private float _greenMaj;
    private readonly Device _device;
    private readonly ClassificationMap _classificationMap = new ClassificationMap();
    internal CustomMap _map;
    internal HiSensitivityChannel SensitivityChannel { get; set; }
    internal float _extendedRangeCL1Multiplier = 1;
    internal float _extendedRangeCL2Multiplier = 1;
    internal float _extendedRangeCL1Threshold = 50000;
    internal float _extendedRangeCL2Threshold = 50000;
    internal bool _extendedRangeEnabled = false;
    private float[] _compensatedCoordinatesCache = { 0,0,0,0 }; //cl0,cl1,cl2,cl3

    public BeadProcessor(Device device)
    {
      _device = device;
    }

    public void SetMap(CustomMap map)
    {
      _map = map;
      _classificationMap.ConstructClassificationMap(_map);
    }

    public ProcessedBead CalculateBeadParams(in RawBead rawBead)
    {
      //The order of operations matters here
      AssignSensitivityChannels(in rawBead);
      CalculateCompensatedCoordinates(in rawBead);
      if (_extendedRangeEnabled)
      {
        CalculateCompensatedCoordinatesForExtendedRange(in rawBead);
      }
      var reg = (ushort) _classificationMap.ClassifyBeadToRegion(in rawBead);
      var rep = CalculateReporter(in reg);
      var zon = (ushort) ClassifyBeadToZone(in _compensatedCoordinatesCache[0]);
      var outBead = new ProcessedBead
      {
        EventTime = rawBead.EventTime,
        fsc_bg = rawBead.fsc_bg,
        vssc_bg = rawBead.vssc_bg,
        cl0_bg = rawBead.cl0_bg,
        cl1_bg = rawBead.cl1_bg,
        cl2_bg = rawBead.cl2_bg,
        cl3_bg = rawBead.cl3_bg,
        rssc_bg = rawBead.rssc_bg,
        gssc_bg = rawBead.gssc_bg,
        greenB_bg = rawBead.greenB_bg,
        greenC_bg = rawBead.greenC_bg,
        greenB = rawBead.greenB,
        greenC = rawBead.greenC,
        l_offset_rg = rawBead.l_offset_rg,
        l_offset_gv = rawBead.l_offset_gv,
        region = reg,
        //fsc = (float)Math.Pow(10, rawBead.fsc),
        fsc = rawBead.fsc,
        violetssc = rawBead.violetssc,
        cl0 = _compensatedCoordinatesCache[0],
        redssc = rawBead.redssc,
        cl1 = _compensatedCoordinatesCache[1],
        cl2 = _compensatedCoordinatesCache[2],
        cl3 = _compensatedCoordinatesCache[3],
        greenssc = rawBead.greenssc,
        reporter = rep,
        zone = zon
      };
      return outBead;
    }

    private void AssignSensitivityChannels(in RawBead rawBead)
    {
      //greenMaj is the hi dyn range channel,
      //greenMin is the high sensitivity channel(depends on filter placement)
      if (SensitivityChannel == HiSensitivityChannel.GreenB)
      {
        _greenMaj = rawBead.greenC;
        _greenMin = rawBead.greenB;
        return;
      }
      _greenMaj = rawBead.greenB;
      _greenMin = rawBead.greenC;
    }

    private void CalculateCompensatedCoordinates(in RawBead outbead)
    {
      var cl1Comp = _greenMaj * _device.Compensation / 100;
      var cl2Comp = cl1Comp * 0.26f;

      var compensatedCl1 = outbead.cl1 - cl1Comp;
      var compensatedCl2 = outbead.cl2 - cl2Comp;

      //Thread unsafe
      _compensatedCoordinatesCache[0] = outbead.cl0;
      _compensatedCoordinatesCache[1] = compensatedCl1;
      _compensatedCoordinatesCache[2] = compensatedCl2;
      _compensatedCoordinatesCache[3] = outbead.cl3;
    }

    private void CalculateCompensatedCoordinatesForExtendedRange(in RawBead outbead)
    {
      //thread unsafe
      var cl1 = _compensatedCoordinatesCache[1];
      var cl2 = _compensatedCoordinatesCache[2];

      if (cl1 > _extendedRangeCL1Threshold)
      {
        _compensatedCoordinatesCache[1] = _extendedRangeCL1Multiplier * outbead.violetssc;
      }

      if (cl2 > _extendedRangeCL2Threshold)
      {
        _compensatedCoordinatesCache[2] = _extendedRangeCL2Multiplier * outbead.cl0;
      }
    }

    private float CalculateReporter(in ushort region)
    {
      var basicReporter = _greenMin > _device.HdnrTrans ? _greenMaj * _device.HDnrCoef : _greenMin;
      var scaledReporter = (basicReporter / _device.ReporterScaling);
      if (!Normalization.IsEnabled || region == 0)
        return scaledReporter;
      var rep = _map.GetFactorizedNormalizationForRegion(in region);
      scaledReporter -= rep;
      if (scaledReporter < 0)
        return 0;
      return scaledReporter;
    }

    private int ClassifyBeadToZone(in float cl0)
    {
      if (!_map.CL0ZonesEnabled)
        return 0;
      //for the sake of robustness. Going from right to left;
      //checks if the value is higher than zone's left boundary.
      //if yes, no need to check other zones
      //check if it falls into the right boundary. else out of any zone
      for (var i = _map.zones.Count - 1; i < 0; i--)
      {
        var zone = _map.zones[i];
        if (cl0 >= zone.Start)
        {
          if (cl0 <= zone.End)
            return zone.Number;
          return 0;
        }
      }
      return 0;
    }
  }
}