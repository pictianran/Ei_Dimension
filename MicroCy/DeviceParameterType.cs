﻿namespace DIOS.Core
{
  public enum DeviceParameterType
  {
    SiPMTempCoeff,
    IdexPosition,
    IdexSteps,
    TotalBeadsInFirmware,
    CalibrationMargin,
    ValveCuvetDrain,
    ValveFan1,
    ValveFan2,
    SyringePosition,
    IsSyringePositionActive,
    PollStepActivity,
    IsInputSelectorAtPickup,
    CalibrationSuccess,
    Pressure,
    PressureAtStartup,
    DNRCoefficient,
    DNRTransition,
    ChannelBias30C,
    SyringeSpeedSheath,
    SyringeSpeedSample,
    MotorX,
    MotorY,
    MotorZ,
    MotorStepsX,
    MotorStepsY,
    MotorStepsZ,
    ChannelTemperature,
    ChannelCompensationBias,
    ChannelOffset,
    Volume,
    GreenAVoltage,
    IsLaserActive,
    LaserPower,
    SystemActivityStatus,
    SheathFlow,
    SampleSyringeStatus,
    NextWellWarning,
    BubbleDetectorStatus,
    ///<summary>For internal use</summary>
    BoardVersion,
    BeadConcentration,
    WellReadingSpeed,
    WellReadingOrder,
    ChannelConfiguration,
    PlateType,
    WashRepeatsAmount,
    AgitateRepeatsAmount
  }
}