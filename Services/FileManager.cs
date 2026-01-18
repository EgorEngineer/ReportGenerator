using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportGenerator.Services
{
    internal class FileManager
    {
        /// Разархивирует ZIP-файл
        /// <param name="zipPath">Путь к архиву</param>
        /// <param name="extractPath">Путь для извлечения</param>
        public void ExtractZip(string zipPath, string extractPath)
        {
            if (Directory.Exists(extractPath))
            {
                Directory.Delete(extractPath, true);
            }

            ZipFile.ExtractToDirectory(zipPath, extractPath);
        }

        /// Создает директорию, если она не существует
        /// <param name="path">Путь к директории</param>
        public void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}