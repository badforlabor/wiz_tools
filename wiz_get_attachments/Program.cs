/**
 * Auth :   liubo
 * Date :   2023-05-27 20:56:20
 * Comment: 获取所有的附件文件
 */


class Program
{
    static void Main(string[] args)
    {
        var root = @"D:\green\WizNote\My Knowledge\Data\xxx";

        var attachments = new List<string>();
        var folders = Directory.GetDirectories(root, "*", SearchOption.AllDirectories);
        foreach (var it in folders)
        {
            if (it.EndsWith("_Attachments"))
            {
                var files = Directory.GetFiles(it, "*", SearchOption.AllDirectories);
                attachments.AddRange(files);
            }
        }

        var dst = @"G:\_tempSSD\wiz_attachment";
        foreach (var it in attachments)
        {
            var rel = it.Substring(root.Length + 1);
            CopyFile(it, Path.Combine(dst, rel));
        }
    }

    static void CopyFile(string src, string dst)
    {
        var folder = Path.GetDirectoryName(dst);
        Directory.CreateDirectory(folder);
        
        File.Copy(src, dst, true);
    }
}