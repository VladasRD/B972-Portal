using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Collections;
using Microsoft.EntityFrameworkCore;
using Box.Common.Web;
using Box.CMS.Models;
using Box.Common;
using System.Security.Claims;

namespace Box.CMS.Services
{

     public enum FileStorages
    {
        Database,
        FileSystem
    }

    public partial class CMSService
    {

         public void VerifyAuthorizationToEditFiles(ClaimsPrincipal user) {

            if (user.IsInRole("CMS_FILE.WRITE"))
                 return;

            throw new System.Security.SecurityException("Not autorized to access content");
        }

        public IEnumerable<File> GetFiles(string filter, int skip, int top, string folder, bool unUsed, OptionalOutTotalCount totalCount = null) {
            
            IQueryable<File> files = _context.Files;

            if (unUsed)
                files = files.Where(x => !_context.ContentDatas.Where(c => c.JSON.Contains(x.FileUId)).Any() && !_context.ContentHeads.Where(w => w.ThumbFilePath.Contains(x.FileUId)).Any());

            if (!String.IsNullOrEmpty(filter)) {
                filter = filter.ToLower();
                files = files.Where(f => f.FileName.ToLower().Contains(filter));
            }

            if (folder != null)
                files = files.Where(f => f.Folder == folder);

            files = files.OrderBy(f => f.FileName);

            if (totalCount != null)
            {
                totalCount.Value = files.Count();
            }

            if (skip != 0)
                files = files.Skip(skip);

            if (top != 0)
                files = files.Take(top);


            return files.ToArray();

            
        }
    
        public File GetFile(string fileUId, bool includeData = true) {
        
            IQueryable<File> file = _context.Files;

            if (includeData)
                file = _context.Files.Include("Data");

            var f = file.SingleOrDefault(x => x.FileUId == fileUId);

            if (EncryptFiles && f.Data!=null)
            {
                var crypt = new CryptUtil(Settings);
                if(f.Data.StoredData!=null)
                    f.Data.StoredData = crypt.DecryptBytes(f.Data.StoredData);

                if (f.Data.StoredThumbData != null)
                    f.Data.StoredThumbData = crypt.DecryptBytes(f.Data.StoredThumbData);
            }

            return f;
        
    }

    public void SaveFile(File file, FileStorages storage) {
        
        var crypt = new CryptUtil(Settings);

        file.Data.StoredData = EncryptFiles ? crypt.EncryptBytes(file.Data.StoredData) : file.Data.StoredData;
        file.Data.StoredThumbData = EncryptFiles ? crypt.EncryptBytes(file.Data.StoredThumbData) : file.Data.StoredThumbData;

        file._CreateDate = DateTime.Now;

        var oldfile = _context.Files.SingleOrDefault(f => f.FileUId == file.FileUId);
        if (oldfile == null) {
            _context.Files.Add(file);
        } else {
            _context.Files.Remove(oldfile);
            _context.Files.Add(file);
        }
        _context.SaveChanges();        
        _log.Log($"File '{file.FileUId}' was uploaded to folder '{file.Folder}'.", saveParameters:false);
    }
    
    public void RemoveFile(string fileUId) {        
        File file = _context.Files.SingleOrDefault(f => f.FileUId == fileUId);
        if (file == null)
            return;
        _context.Files.Remove(file);
        _context.SaveChanges();        
        _log.Log($"File '{file.FileUId}' was deleted from folder '{file.Folder}'.", saveParameters:false);
    }

    public void RemoveUnusedFiles() {        
        IQueryable<File> files = _context.Files.Where(x => !_context.ContentDatas.Where(c => c.JSON.Contains(x.FileUId)).Any() 
        && !_context.ContentHeads.Where(w => w.ThumbFilePath.Contains(x.FileUId)).Any());
        _context.Files.RemoveRange(files);
        _context.SaveChanges();                
    }


    public File GetFileThumb(string fileUId) {     

        var f = _context.Files.Where(x => x.FileUId == fileUId).SingleOrDefault();
        var thumbData = _context.FileData.Where(x => x.FileUId == fileUId).Select(x => new { bytes = x.StoredThumbData }).SingleOrDefault();

        if(thumbData!=null) {
            f.Data = new FileData() { FileUId = f.FileUId, StoredThumbData = thumbData.bytes };
        }
        
        if (EncryptFiles && f.Data != null && f.Data.StoredThumbData != null) {
            var crypt = new CryptUtil(Settings);                    
            f.Data.StoredThumbData = crypt.DecryptBytes(f.Data.StoredThumbData);
        }

        return f;
     
    }    

    public byte[] GetScaledImageFile(byte[] bytes, double scale = 1, int xdes = 0, int ydes = 0, int finalW = 0, int finalH = 0, string mimeType = null) {

        if (scale == 1 && xdes == 0 && ydes == 0)
            return bytes;

        System.IO.MemoryStream stream = new System.IO.MemoryStream(bytes);
        System.Drawing.Image image = System.Drawing.Image.FromStream(stream);

        int height = (int)(image.Height * scale);
        int width = (int)(image.Width * scale);

        if (finalW == 0)
            finalW = width;

        if (finalH == 0)
            finalH = height;

        System.Drawing.Bitmap newImg = new System.Drawing.Bitmap(finalW, finalH);
        System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newImg);
        g.DrawImage(image, new System.Drawing.Rectangle(-xdes, -ydes, width, height));

        g.Dispose();

        return ImageToBytes(newImg, mimeType);
    }

    public byte[] GetImageFileThumb(byte[] bytes, int width, int height, int maxWidth, int maxHeight, string vAlign = "center", string hAlign = "center", string mimeType = null, string mode = null) {

        System.IO.MemoryStream stream = new System.IO.MemoryStream(bytes);
        System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
        stream.Close();

        float rtX = width / (float)image.Width;
        float rtY = height / (float)image.Height;

        if (String.IsNullOrEmpty(mode)) {
            if (height == 0) {
                height = (int)(image.Height * rtX);
            }
            if (width == 0) {
                width = (int)(image.Width * rtY);
            }

            if (maxWidth == 0)
                maxWidth = width;

            if (maxHeight == 0)
                maxHeight = height;

            if (height < maxHeight)
                maxHeight = height;

            if (width < maxWidth)
                maxWidth = width;
        }

        if (mode == "f" || mode == "fill") {
            maxWidth = width;
            maxHeight = height;

            if (rtY < rtX)
                height = (int)(image.Height * rtX);
            else
                width = (int)(image.Width * rtY);

        }

        if (String.IsNullOrEmpty(vAlign))
            vAlign = "center";

        if (String.IsNullOrEmpty(hAlign))
            hAlign = "center";

        int top = 0;
        if (vAlign == "bottom")
            top = (maxHeight - height);
        if (vAlign == "center")
            top = (maxHeight - height) / 2;

        int left = 0;
        if (hAlign == "right")
            left = (maxWidth - width);
        if (hAlign == "center")
            left = (maxWidth - width) / 2;

        System.Drawing.Bitmap newImg = new System.Drawing.Bitmap(maxWidth, maxHeight);
        System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newImg);
        g.DrawImage(image, new System.Drawing.Rectangle(left, top, width, height));
        g.Dispose();

        return ImageToBytes(newImg, mimeType);

    }

    private byte[] ImageToBytes(System.Drawing.Image img, string mimeType) {

        var imgType = System.Drawing.Imaging.ImageFormat.Jpeg;
        switch (mimeType) {
            case "image/png":
                imgType = System.Drawing.Imaging.ImageFormat.Png;
                break;
            case "image/gif":
                imgType = System.Drawing.Imaging.ImageFormat.Gif;
                break;
            case "image/tiff":
                imgType = System.Drawing.Imaging.ImageFormat.Tiff;
                break;
            case "image/bmp":
                imgType = System.Drawing.Imaging.ImageFormat.Bmp;
                break;
            case "image/icon":
                imgType = System.Drawing.Imaging.ImageFormat.Icon;
                break;
            default:
                imgType = System.Drawing.Imaging.ImageFormat.Jpeg;
                break;
        }

        byte[] byteArray = new byte[0];
        using (var stream = new System.IO.MemoryStream()) {
            img.Save(stream, imgType);
            stream.Close();
            byteArray = stream.ToArray();
        }
        return byteArray;
    }

    public void SetFileThumb(File file) {
        if (file.Type.StartsWith("image")) {
            file.Data.StoredThumbData = GetImageFileThumb(file.Data.StoredData, CMSThumbWidth, CMSThumbHeight, 0, 0, "image/jpeg");
        }        
        else {            
            file.Data.StoredThumbData = GetDocumentThumb(file.FileName);
        }
    }

    public byte[] GetDocumentThumb(string fileName) {
        string iconFile = "document";

        int dotIndex = fileName.LastIndexOf(".");
        if (dotIndex >= 0) {
            string ext = fileName.Substring(dotIndex);
            switch (ext) {
                case ".xls":
                case ".xlsx":
                case ".csv":
                    iconFile = "xls";
                    break;
                case ".doc":
                case ".docx":
                    iconFile = "doc";
                    break;
                case ".ppt":
                case ".pptx":
                    iconFile = "doc";
                    break;
                case ".mp3":
                    iconFile = "mp3";
                    break;
                case ".pdf":
                    iconFile = "pdf";
                    break;
                case ".zip":
                    iconFile = "zip";
                    break;
            }
        }

        var path = System.IO.Path.Combine(WebPath, "images/cms/file-icons/", iconFile + ".png");
        return System.IO.File.ReadAllBytes(path);
    }

    public string CleanFileName(string name) {
        string cleanName = name.Replace("\"", string.Empty);
        int idxSlash = cleanName.LastIndexOf("\\");
        if (idxSlash > 0)
            cleanName = cleanName.Substring(idxSlash + 1);
        return cleanName;
    }

    private int CMSThumbWidth {
        get {
            if(Settings.CMS_THUMB_WIDTH==0)
                return 150;
            return Settings.CMS_THUMB_WIDTH;            
        }
    }

    private int CMSThumbHeight {
        get {
            if(Settings.CMS_THUMB_HEIGHT==0)
                return 150;
            return Settings.CMS_THUMB_HEIGHT;
        }
    }

    private bool EncryptFiles
    {
        get
        {            
            object i = Settings.ENCRYPT_FILES;
            if (i == null)
                return false;

            bool value = true;
            Boolean.TryParse(i.ToString(), out value);

            return value;
        }
    }

    
    }



}
