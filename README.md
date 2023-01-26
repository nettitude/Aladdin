# Aladdin
```
           .-.
          [.-''-.,
          |  //`~\)
          (<| 0\0|>_
          ";\  _"/ \\_ _,
         __\|'._/_  \ '='-,
        /\ \    || )_///_\>>
       (  '._ T |\ | _/),-'
        '.   '._.-' /'/ |
        | '._   _.'`-.._/
        ,\ / '-' |/
        [_/\-----j
   _.--.__[_.--'_\__
  /         `--'    '---._
 /  '---.  -'. .'  _.--   '.
 \_      '--.___ _;.-o     /
   '.__ ___/______.__8----'
     c-'----'
  Lefty @lefterispan - Nettitude Red Team - 2022 / 2023 
```

# About

Aladdin is a payload generation technique based on the work of James Forshaw (@tiraniddo) that allows the deseriallization of a .NET payload and execution in memory. The original vector was documented on https://www.tiraniddo.dev/2017/07/dg-on-windows-10-s-executing-arbitrary.html.

By spawning the process `AddInProcess.exe` with arguments ```/guid:32a91b0f-30cd-4c75-be79-ccbd6345de99``` and ```/pid:```, the process will start a named pipe under `\\.\pipe\32a91b0f-30cd-4c75-be79-ccbd6345de99` and will wait for a .NET Remoting object. If we generate a payload that has the appropiate packet bytes required to communicate with a .NET remoting listener we will be able to trigger the ActivitySurrogateSelector class from System.Workflow.ComponentModel. and gain code execution.

Originally, James Forshaw released a POC at ```https://github.com/tyranid/DeviceGuardBypasses/tree/master/CreateAddInIpcData```. However this POC will fail on recent versions of Windows since Microsoft went ahead and patched the vulnerable System.Workflow.ComponentModel (https://github.com/microsoft/dotnet-framework-early-access/blob/master/release-notes/NET48/dotnet-48-changes.md).

Nick Landers (@monoxgas) however, identified a way to disable the check that Microsoft introduced and wrote a detailed article at https://www.netspi.com/blog/technical/adversary-simulation/re-animating-activitysurrogateselector/ . The bypass is documented at https://github.com/pwntester/ysoserial.net/pull/41 .

Aladdin is a payload generation tool, which using the specific bypass as well as the necessary header bytes of the .NET remoting protocol is able to generate initial access payloads that abuse the `AddInProcess` as originally documented. 

The provided templates are:

    * HTA

    * VBA

    * JS
    
    * CHM

## Notes

In order for the attack to be successfull the .NET assembly must contain a single public class with an empty constructor to act as the entry point during deserialization. An example assembly has been included in the project.
```
public class EntryPoint {
    public EntryPoint() {
        MessageBox.Show("Hello");
    }
}
```

## Usage

```
Usage:
  -w, --scriptType=VALUE     Set to js / hta / vba / chm.

  -o, --output=VALUE         The generated output, e.g: -o
                               C:\Users\Nettitude\Desktop\payload

  -a, --assembly=VALUE       Provided Assembly DLL, e.g: -a
                               C:\Users\Nettitude\Desktop\popcalc.dll

  -h, --help                 Help

```

## OpSec

* The user supplied .NET binary will be executed under the `AddInProcess.exe` that gets spawned from the HTA / JS payload. The spawning of the processes currently happens using the 9BA05972-F6A8-11CF-A442-00A0C90A8F39 COM object (https://dl.packetstormsecurity.net/papers/general/abusing-objects.pdf) which will launch the process as a child of `Explorer.exe` process.

* The GUID supplied in the process parameters of `AddInProcess.exe` can be user controlled. At the moment the guid is hardcoded in the template and the code.

* CHM executes the JScript through XSLT transformation

## Defensive Considerations

* `Addinprocess.exe` will always launch with `/guid` and `/pid`. Baseline your environment for legitimate uses - monitor the rest

## Useful References:

    * https://www.tiraniddo.dev/2017/07/dg-on-windows-10-s-executing-arbitrary.html

    * https://www.netspi.com/blog/technical/adversary-simulation/re-animating-activitysurrogateselector/

## Readme / Credits
Code is based on the following repos:

    * https://github.com/tyranid/DeviceGuardBypasses/tree/master/CreateAddInIpcData

    * https://github.com/pwntester/ysoserial.net


Shouts to: 
* @m0rv4i for helping with C# nuances
* @ace0fspad3s for troubleshooting
* @ Nettitude RT for being awesome

