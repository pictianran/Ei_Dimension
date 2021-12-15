﻿using System;
using MadWizard.WinUSBNet;

namespace MicroCy
{
  public class USBConnection : ISerial
  {
    public byte[] InputBuffer { get; }
    public bool IsActive { get; private set; }
    private USBDevice USBDevice;
    private const string InterfaceGuid = "F70242C7-FB25-443B-9E7E-A4260F373982"; // interface GUID, not device guid

    public USBConnection()
    {
      InputBuffer = new byte[512];
      _ = Init();
    }

    public bool Init()
    {
      USBDeviceInfo[] di = USBDevice.GetDevices(InterfaceGuid);   // Get all the MicroCy devices connected
      if (di.Length > 0)
      {
        try
        {
          USBDevice = new USBDevice(di[0].DevicePath);     // just grab the first one for now, but should support multiples
          Console.WriteLine(string.Format("{0}:{1}", USBDevice.Descriptor.FullName, USBDevice.Descriptor.SerialNumber));
          IsActive = true;
        }
        catch { return false; }
        return true;
      }
      else
      {
        Console.WriteLine("USB devices not found");
        return false;
      }
    }

    public void Write(byte[] buffer)
    {
      USBDevice.Interfaces[0].OutPipe.Write(buffer);
    }

    public void BeginRead(AsyncCallback func)
    {
      if (IsActive)
        _ = USBDevice.Interfaces[0].Pipes[0x81].BeginRead(InputBuffer, 0, InputBuffer.Length, new AsyncCallback(func), null);
    }

    public void Read()
    {
      if (IsActive)
        USBDevice.Interfaces[0].Pipes[0x81].Read(InputBuffer, 0, InputBuffer.Length);
    }

    public void EndRead(IAsyncResult result)
    {
      _ = USBDevice.Interfaces[0].Pipes[0x81].EndRead(result);
    }
  }
}
