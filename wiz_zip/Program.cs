using Ionic.Zip;

/**
 * Auth :   liubo
 * Date :   2023-05-27 20:56:20
 * Comment: zip所有笔记文件夹
 */


class Program
{
    static void Main(string[] args)
    {
        var root = @"G:\_tempSSD\wiz";

        var folders = Directory.GetDirectories(root);
        foreach (var it in folders)
        {
            if (it.EndsWith("_split"))
            {
                continue;
            }

            try
            {
                File.Delete(it + ".zip");
            }
            catch (Exception e)
            {
                
            }
            
            ZipFolder(it, it + ".zip");
        }

    }

    public static void ZipFolder(string folder, string zipFileName)
    {
        if (!Directory.Exists(folder))
        {
            return;
        }

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(zipFileName));
        }
        catch (Exception e)
        {
                
        }

        using (ZipFile zip = new ZipFile())
        {
            zip.UseUnicodeAsNecessary= true;  // utf-8
            zip.AddDirectory(folder);
            // zip.AddDirectory(folder, Path.GetFileName(folder));
            // zip.Comment = "This zip was created at " + System.DateTime.Now.ToString("G") ; 
            zip.Save(zipFileName);
        }
    }
}