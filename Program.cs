using System;
using System.IO;
using System.Management;
using OpenHardwareMonitor.Hardware;

class ComputerInfoProgram
{
    public static void Main()
    {
        Console.Title = "Computer Info Program";
        Console.WriteLine("ComputerInfoProgram 1.0V 메뉴");
        Console.WriteLine("번호를 쓰세요.");
        Console.WriteLine("1. 드라이브");
        Console.WriteLine("2. 운영체제");
        Console.WriteLine("3. 컴퓨터 부품");
        string userInput = Console.ReadLine();

        switch (userInput)
        {
            case "1":
                Drive();
                break;
            case "2":
                Computer();
                break;
            case "3":
                ComputerPart();
                break;
            case "4":
                return;
            default:
                Console.WriteLine("잘못된 입력입니다. 다시 시도해주세요.");
                break;
        }
    }
    public static void Drive()
    {
        Console.WriteLine("DriveOutput 1.0V");
        DriveInfo[] drives = DriveInfo.GetDrives();

        foreach (DriveInfo drive in drives)
        {
            Console.WriteLine("드라이브 문자 : {0}", drive.Name);
            Console.WriteLine("드라이브 종류 : {0}", drive.DriveType);
            if (drive.IsReady == true)
            {
                Console.WriteLine("     드라이브 이름 : {0}", drive.VolumeLabel);
                Console.WriteLine("     드라이브 파일 시스템 : {0}", drive.DriveFormat);
                Console.WriteLine("     드라이브 크기 : {0} GB", BytesToGigabytes(drive.TotalSize));
                Console.WriteLine("     드라이브 남은 공간 : {0} GB", BytesToGigabytes(drive.TotalFreeSpace));
            }
        }
        Main();
    }
    public static void Computer()
    {
        Console.WriteLine("ComputerOutput 1.0V");
        Console.WriteLine("운영체제 버전 : {0}", Environment.OSVersion);
        Console.WriteLine("운영체제 64비트 : {0}", Environment.Is64BitOperatingSystem);
        Main();
    }
    public static void ComputerPart()
    {
        Console.WriteLine("ComputerPartOutput 1.0V");
        Computer computer = new Computer();
        computer.CPUEnabled = true;
        computer.Open();
        int totalsize = 0;
        int freesize = 0;
        ManagementClass manaclass = new ManagementClass("Win32_OperatingSystem");
        ManagementObjectCollection basepart = manaclass.GetInstances();
        foreach (ManagementObject obj in basepart)
        {
            totalsize = int.Parse(obj["TotalVisibleMemorySize"].ToString());
            freesize = int.Parse(obj["FreePhysicalMemory"].ToString());
        }
        foreach (var hardware in computer.Hardware)
        {
            if (hardware.HardwareType == HardwareType.CPU)
            {
                hardware.Update();
                foreach (var sensor in hardware.Sensors)
                {
                    if (sensor.SensorType == SensorType.Load)
                    {
                        Console.WriteLine("CPU 코어 #{0} 불러오기: {1}% ", sensor.Index + 1, sensor.Value.HasValue ? sensor.Value.Value.ToString() : "no value");
                    }
                }
            }
        }
        totalsize = totalsize / 1024;
        freesize = freesize / 1024;

        Console.WriteLine("메모리 크기 : {0} MB", totalsize);
        Console.WriteLine("남은 메모리 공간 : {0} MB", freesize);
        computer.Close();
        Main();
    }
    static double BytesToGigabytes(long bytes)
    {
        return (double)bytes / 1024 / 1024 / 1024;
    }
}
