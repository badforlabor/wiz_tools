
/**
 * Auth :   liubo
 * Date :   2023-05-27 19:27:09
 * Comment: 同步为知笔记到语雀
 *     1. 为知笔记中，文件->导出文件->导出html
 *     2. 用此工具，处理对应的文件夹，会生成xxx_split/xxx.zip
 *     3. 在语雀中，导入->evernote(zip)，然后选择zip包即可
 */


using System.Diagnostics;
using System.Text.RegularExpressions;
using Ionic.Zip;

class Program
{
    private const string HTML_FILE = "*.html";
    private const string SPLIT_POSTFIX = "_split";
    private const string HTML_FOLDER_POSTFIX = "_files";
    private const int COUNTER_PER_ZIP = 20;
    
    static void Main(string[] args)
    {
        var root = @"G:\_tempSSD\wiz\";
        if (args.Length > 0)
        {
            root = args[0];
        }
        root = root.TrimEnd('\\', '/');
        Console.WriteLine($"处理文件夹：{root}");

        var folders = Directory.GetDirectories(root);
        bool bNoteFolder = false;
        foreach (var folder in folders)
        {
            if (folder.EndsWith(HTML_FOLDER_POSTFIX))
            {
                bNoteFolder = true;
                break;
            }
        }

        if (bNoteFolder)
        {
            ProcOneNote(root);
        }
        else
        {
            ProcAllNote(root);
        }
    }

    static void ProcAllNote(string allNotePath)
    {
        var folders = Directory.GetDirectories(allNotePath);
        foreach (var folder in folders)
        {
            if (folder.EndsWith(HTML_FOLDER_POSTFIX))
            {
                throw new Exception("操作的目录不对！");
            }
        }

        foreach (var folder in folders)
        {
            if (folder.EndsWith(SPLIT_POSTFIX))
            {
                continue;
            }

            if (Directory.Exists($"{folder}{SPLIT_POSTFIX}"))
            {
                continue;
            }

            ProcOneNote(folder);
        }
    }

    static void ProcOneNote(string noteFullPath)
    {
        var src = Path.GetFullPath(noteFullPath);
        src = src.TrimEnd('\\', '/');
        Console.WriteLine($"处理文件夹：{src}");
        
        var dst = src + SPLIT_POSTFIX;
        try
        {
            Directory.Delete(dst, true);
        }
        catch (Exception e)
        {
            
        }

        int lastCount = 0;
        ProcFolder(dst,src, "./", ref lastCount);
        
        // 检查一下
        var srcFiles = Directory.GetFiles(src, HTML_FILE, SearchOption.AllDirectories);
        var dstFiles = Directory.GetFiles(dst, HTML_FILE, SearchOption.AllDirectories);
        Debug.Assert(srcFiles.Length == dstFiles.Length);

        // 压缩zip文件夹
        var folders = Directory.GetDirectories(dst);
        foreach (var folder in folders)
        {
            var zipFile = $"{folder}.zip";
            try
            {
                File.Delete(zipFile);
            }
            catch (Exception e)
            {
                
            }
            ZipFolder(folder, zipFile);
        }
    }

    static string FormatFolderPath(string root)
    {
        root = Path.GetFullPath(root);
        root = root.TrimEnd('/', '\\');
        return root;
    }

    static void ProcFolder(string dst, string root, string childFolder, ref int lastCount)
    {
        root = FormatFolderPath(root);
        
        var src = Path.Combine(root, childFolder);
        src = FormatFolderPath(src);

        var files = Directory.GetFiles(src, HTML_FILE, SearchOption.TopDirectoryOnly);
        
        var fileList = new List<string>(files);
        fileList.Sort();

        for (int i = 0; i < fileList.Count; i++)
        {
            lastCount++;
            var dstFolder = Path.Combine(dst, $"{lastCount / COUNTER_PER_ZIP}");
            Directory.CreateDirectory(dstFolder);
            
            // 将1.html转化成1_files
            var file = fileList[i];
            var fileName = file.Substring(src.Length + 1);
            var folder = fileName;
            folder = folder.Substring(0, folder.Length - 5);
            folder = folder + HTML_FOLDER_POSTFIX;

            if (Directory.Exists(Path.Combine(src, folder)))
            {
                CopyFolder(Path.Combine(src, folder), Path.Combine(dstFolder, folder));
            }
            
            File.Copy(file, Path.Combine(dstFolder, fileName), true);
        }

        var childFullPath = FormatFolderPath(Path.Combine(root, childFolder));
        var folders = Directory.GetDirectories(childFullPath);
        foreach (var f in folders)
        {
            if (f.EndsWith(HTML_FOLDER_POSTFIX))
            {
                continue;
            }

            var folderName = f.Substring(childFullPath.Length + 1);

            // 换一个文件夹，防止重名情况
            lastCount = ((lastCount / COUNTER_PER_ZIP) + 1) * COUNTER_PER_ZIP;
            
            // 子文件夹
            ProcFolder(dst, root, Path.Combine(childFolder, folderName), ref lastCount);
        }
    }

    public static void CopyFolder(string src, string dst)
    {
        var fullpath = Path.GetFullPath(src);
        if (!Directory.Exists(fullpath))
        {
            return;
        }

        if (!Directory.Exists(dst))
        {
            Directory.CreateDirectory(dst);
        }

        //Now Create all of the directories
        foreach (string dirPath in Directory.GetDirectories(fullpath, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dirPath.Replace(fullpath, dst));
        }

        //Copy all the files & Replaces any files with the same name
        foreach (string newPath in Directory.GetFiles(fullpath, "*.*",SearchOption.AllDirectories))
        {
            File.Copy(newPath, newPath.Replace(fullpath, dst), true);
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