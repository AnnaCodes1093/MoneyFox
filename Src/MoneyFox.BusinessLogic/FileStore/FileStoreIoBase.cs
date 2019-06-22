﻿using System;
using System.IO;
using NLog;

namespace MoneyFox.BusinessLogic.FileStore
{
    public class FileStoreIoBase : FileStoreBase
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        public FileStoreIoBase(string basePath)
        {
            BasePath = basePath;
        }

        protected string BasePath { get; }

        public override Stream OpenRead(string path)
        {
            var fullPath = AppendPath(path);
            if (!File.Exists(fullPath))
            {
                return null;
            }

            return File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        public override bool TryMove(string @from, string to, bool overwrite)
        {
            try
            {
                var fullFrom = AppendPath(from);
                var fullTo = AppendPath(to);

                if (!File.Exists(fullFrom))
                {
                    logger.Error("Error during file move {0} : {1}. File does not exist!", from, to);
                    return false;
                }

                if (File.Exists(fullTo))
                {
                    if (overwrite)
                    {
                        File.Delete(fullTo);
                    } else
                    {
                        return false;
                    }
                }

                File.Move(fullFrom, fullTo);
                return true;
            } 
            catch (Exception ex)
            {
                logger.Error(ex.ToString);return false;
            }
        }

        protected override void WriteFileCommon(string path, Action<Stream> streamAction)
        {
            var fullPath = AppendPath(path);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            using (var fileStream = File.OpenWrite(fullPath))
            {
                streamAction?.Invoke(fileStream);
            }
        }
        
        protected virtual string AppendPath(string path)
            => Path.Combine(BasePath, path);
    }
}
