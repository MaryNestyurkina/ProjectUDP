using System;
using WixSharp;
using System.Collections.Generic;

namespace MsiFlashDataConverter
{
    static class Extensions
    {
        public static void ModifyEach<T>(this IList<T> source, Func<T, T> projection)
        {
            for (int i = 0; i < source.Count; i++)
            {
                source[i] = projection(source[i]);
            }
        }
    }
    class Program
    {
        
        static void Main()
        {
            string artifactsFolder = Environment.GetEnvironmentVariable("WORKSPACE");
            StLib.DeployUtils.DirectoryDescriptor dirDesc = new StLib.DeployUtils.DirectoryDescriptor(artifactsFolder).GetEntries();

            List<string> dirrectories = dirDesc.GetChildDirs();

            dirrectories.ModifyEach(x => x.Replace(artifactsFolder + "\\", ""));  //.ForEach(v => = v.Replace(artifactsFolder, ""));
            //dirrectories.ForEach((v) => v = v.Replace("\\", "\\\\"));

            WixEntity[] items = new WixEntity[dirrectories.Count];
            for (int i = 0; i != dirrectories.Count; i++)
            {

                items[i] =  new WixSharp.File(dirrectories[i]+"\\a.txt");
            }
            Dir baseDir = new Dir("D:\\deploy", new File("FlashDataConverter.exe"), new Dir("a", new File("a\\a.txt"), new Dir("b", new File("a\\b\\a.txt"))));

            Dir[] dirrs = new Dir[1];
            dirrs[0] = baseDir; // new Dir("D:\\deploy", items);


            var project = new Project();
            project.SourceBaseDir = artifactsFolder;
            project.Name = "FlashDataConverter";
            project.Dirs = dirrs;
            //project.Dirs.Add(
            //project.
            project.OutFileName = "flashdataconverter";

            Dir[] d = project.AllDirs;

            project.Version = new Version(1,0,13,0);
            
            project.GUID = new Guid("6fe30b47-2577-43ad-9095-1861ba25889c");
            //project.SourceBaseDir = "<input dir path>";
            //project.OutDir = "<output dir path>";
            project.MajorUpgradeStrategy = new MajorUpgradeStrategy
            {
                UpgradeVersions = VersionRange.OlderThanThis,
                PreventDowngradingVersions = VersionRange.NewerThanThis,
                NewerProductInstalledErrorMessage = "Newer version already installed",
                A
            };

            project.BuildMsi();
        }
    }
}
