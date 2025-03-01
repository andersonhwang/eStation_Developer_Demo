using Demo_Common.Entity;
using Demo_Common.Enum;
using Demo_Common.Helper;
using Demo_Common.Service;
using Demo_WPF.Helper;
using Demo_WPF.Model;
using Microsoft.Win32;
using Serilog;
using SkiaSharp;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;

namespace Demo_WPF.ViewModel
{
    public class TaskEslViewModel : DialogViewModelBase
    {
        private TagType type;
        private TaskESL esl = new();
        /// <summary>
        /// Tag type
        /// </summary>
        public TagType Type { get => type; set { type = value; NotifyPropertyChanged(nameof(Type)); } }
        /// <summary>
        /// Task esl
        /// </summary>
        public TaskESL Esl { get => esl; set { esl = value; NotifyPropertyChanged(nameof(Esl)); } }
        /// <summary>
        /// Command - browse
        /// </summary>
        public ICommand CmdBrowse { get; set; }
        /// <summary>
        /// Command - publish
        /// </summary>
        public ICommand CmdPublish { get; set; }
        /// <summary>
        /// Send handler
        /// </summary>
        public Action<string[]> SendHandler;

        /// <summary>
        /// Constructor 
        /// </summary>
        public TaskEslViewModel()
        {
            CmdBrowse = new MyCommand(DoBrowse, CanBrowse);
            CmdPublish = new MyAsyncCommand(DoPublish, CanPublish);

            Esl.Pattern = Pattern.UpdateDisplay;
            Esl.PageIndex = PageIndex.P0;
        }

        /// <summary>
        /// Set tags
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="tags">Tags</param>
        /// <param name="action">Action</param>
        public void SetTags(int token, string[] tags, Action<string[]> action)
        {
            Esl.ID = tags;
            Esl.Token = token;
            SendHandler = action;
            Type = TagHelper.GetTagType(tags[0]);
        }

        /// <summary>
        /// Do browse
        /// </summary>
        /// <param name="obj"></param>
        private void DoBrowse(object obj)
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    RestoreDirectory = true,
                    DefaultDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName),
                    DefaultExt = ".bmp",
                    Filter = "Bitmap|*.bmp"
                };

                bool? result = dialog.ShowDialog();
                if (result != true) return;
                var bitmap = SKBitmap.Decode(dialog.FileName);

                Esl.Path = dialog.FileName;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Load_Firmware_Error");
            }
        }

        /// <summary>
        /// Do publish
        /// </summary>
        /// <param name="arg">Parameter</param>
        /// <returns>The task</returns>
        private async Task DoPublish(object arg)
        {
            var result = SendResult.Unknown;
            var bytes = GetImageData(Esl.Path, Type);
            switch (arg.ToString())
            {
                case "0":
                    var list = new List<ESLEntity1>();
                    var base64 = System.Convert.ToBase64String(bytes);
                    foreach (var id in Esl.ID)
                    {
                        if (PackageEntity(new ESLEntity1 { TagID = id, Base64String = base64 }) is not ESLEntity1 esl) continue;
                        list.Add(esl);
                    }
                    result = await SendService.Instance.Send(0x02, "taskESL", list);
                    break;
                case "1":
                    var list2 = new List<ESLEntity2>();
                    foreach (var id in Esl.ID)
                    {
                        if (PackageEntity(new ESLEntity2 { TagID = id, Bytes = bytes }) is not ESLEntity2 esl) continue;
                        list2.Add(esl);
                    }
                    result = await SendService.Instance.Send(0x03, "taskESL2", list2);
                    break;
            }

            if (result == SendResult.Success)
            {
                SendHandler?.Invoke(Esl.ID);
            }
            else
            {
                MsgHelper.Error("Publish_Task_Error:" + result.ToString());
            }
            DialogResult = true;
        }

        /// <summary>
        /// Can browse
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>True</returns>
        private bool CanBrowse(object arg) => true;

        /// <summary>
        /// Can publish
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Result</returns>
        private bool CanPublish(object obj)
        {
            if(Esl.Pattern.Equals(Pattern.UpdateDisplay) || Esl.Pattern.Equals(Pattern.Update)) return File.Exists(Esl.Path);
            return true;
        }

        /// <summary>
        /// Package entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private ESLEntity PackageEntity(ESLEntity entity)
        {
            entity.Pattern = Esl.Pattern;
            entity.PageIndex = Esl.PageIndex;
            entity.Token = Esl.Token;
            entity.R = Esl.R;
            entity.G = Esl.G;
            entity.B = Esl.B;
            entity.Times = Esl.Times;
            entity.NewKey = Esl.NewKey;
            return entity;
        }

        /// <summary>
        /// Get image data
        /// </summary>
        /// <param name="path">Image path</param>
        /// <param name="type">Tag type</param>
        /// <returns>Image data</returns>
        private byte[] GetImageData(string path, TagType type)
        {
            try
            {
                if(Path.Exists(path) is false) return [];
                var bitmap = SKBitmap.Decode(path);
                if (bitmap.Info.Width.Equals(type.Width) && bitmap.Info.Height.Equals(type.Height))
                    return bitmap.Bytes;
                var infor = new SKImageInfo(W8(type.Width), type.Height);
                using var source = SKImage.FromBitmap(bitmap);
                using var surface = SKSurface.Create(infor);
                surface.Canvas.Clear(SKColors.White);
                surface.Canvas.DrawImage(source, source.Info.Rect, source.Info.Rect);
                using var image = surface.Snapshot();
                using var result = SKBitmap.FromImage(image);
                return result.Bytes;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "IMG_DAT_ERR");
                return [];
            }

            static int W8(int w) => (w % 8) == 0 ? w : (((w / 8) + 1) * 8);
        }
    }
}
