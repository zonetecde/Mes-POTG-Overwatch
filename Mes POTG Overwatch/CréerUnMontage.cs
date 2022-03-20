using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Splicer;
using Splicer.Timeline;
using Splicer.Renderer;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Mes_POTG_Overwatch
{
    static class CréerUnMontage
    {
        private static void TrimVidéo(string file)
        {
            //ffmpeg - i movie.mp4 - ss 00:00:03 - t 00:00:08 - async 1 cut.mp4

            ExecuteArg(new List<string>()
            {
                "cd " + AppDomain.CurrentDomain.BaseDirectory + "montage",
                "ffmpeg -ss 00:00:05 -i " + Path.GetFileName(file) + " -to 00:00:12 -c copy " + Path.GetFileNameWithoutExtension(file) + "a.mp4"
            });
        }

        public static void CréerMontage(List<string> FilesPaths)
        {
            foreach(string file in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "montage"))
            {
                if (Path.GetExtension(file).Contains(".mp4") )
                    File.Delete(file);
            }

            int i = 0;
            FilesPaths.ForEach(x =>
            {
                File.Copy(x, AppDomain.CurrentDomain.BaseDirectory + @"montage\" + i + ".mp4", true);
                
                TrimVidéo(AppDomain.CurrentDomain.BaseDirectory + @"montage\" + i + ".mp4");
                i++;

                Thread.Sleep(500);
            });

            int i2 = 0;
            FilesPaths.ForEach(x =>
            {
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"montage\" + i2 + ".mp4");
                i2++;
            });

            Thread.Sleep(1000);

            ExecuteArg(new List<string>() 
            { 
                "cd " + AppDomain.CurrentDomain.BaseDirectory + "montage", 
                "(for %i in (*.mp4) do @echo file '%i') > montageList.txt" , 
                "ffmpeg -f concat -i montageList.txt -c copy output.mp4" 
            });

            Process.Start("explorer.exe", AppDomain.CurrentDomain.BaseDirectory + "montage");
            
        }

        private static void ExecuteArg(List<string> args)
        {
            Process p = new Process();
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "cmd.exe";
            info.RedirectStandardInput = true;
            info.UseShellExecute = false;

            p.StartInfo = info;
            p.Start();

            using (StreamWriter sw = p.StandardInput)
            {
                if (sw.BaseStream.CanWrite)
                {
                    args.ForEach(x =>
                    {
                        sw.WriteLine(x);
                    });
                }
            }

        }
    }
}
