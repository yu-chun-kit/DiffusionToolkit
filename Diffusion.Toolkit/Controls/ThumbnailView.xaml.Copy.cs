using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Threading;
using System.Windows;
using Diffusion.Civitai;
using Diffusion.Civitai.Models;
using Diffusion.Toolkit.Models;

namespace Diffusion.Toolkit.Controls
{
    public partial class ThumbnailView
    {
        private async void SearchModel(object obj)
        {
            if (Model.CurrentImage?.ModelHash == null) return;
            
            var hash = Model.CurrentImage.ModelHash;

            using (var client = new CivitaiClient())
            {
                try
                {
                    var modelVersion = await client.GetModelVersionsByHashAsync(hash, CancellationToken.None);

                    Process.Start("explorer.exe", $"\"https://civitai.com/models/{modelVersion.ModelId}?modelVersionId={modelVersion.Id}\"");
                }
                catch (CivitaiRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
                {
                    var message = "The requested model hash was not found";
                    await MessagePopupManager.Show(message, "Search Model", PopupButtons.OK);
                }
            }
        }

        public Action<string> Toast { get; set; }

        private void CopyPath(object obj)
        {
            CopyToClipboard(Model.CurrentImage?.Path, "path", Toast);
        }

        private void CopyPrompt(object obj)
        {
            CopyToClipboard(Model.CurrentImage?.Prompt, "prompt", Toast);
        }

        private void CopyNegative(object obj)
        {
            CopyToClipboard(Model.CurrentImage?.NegativePrompt, "negative prompt", Toast);
        }

        private void CopySeed(object obj)
        {
            CopyToClipboard(Model.CurrentImage?.Seed, "seed", Toast);
        }

        private void CopyHash(object obj)
        {
            CopyToClipboard(Model.CurrentImage?.ModelHash, "hash", Toast);
        }

        private void CopyParameters(object obj)
        {
            if (Model.CurrentImage == null) return;

            CopyParameters(
                Model.CurrentImage.Prompt,
                Model.CurrentImage.NegativePrompt,
                Model.CurrentImage.OtherParameters,
                Toast
            );
        }

        private void CopyOthers(object obj)
        {
            CopyToClipboard(Model.CurrentImage?.OtherParameters, "other parameters", Toast);
        }

        public static void CopyToClipboard<T>(T value, string itemName, Action<string> toastAction)
        {
            if (value == null) return;

            string stringValue = value is string s ? s : value.ToString();
            CopyToClipboardInternal(stringValue, itemName, toastAction);
        }

        public static void CopyParameters(string prompt, string negativePrompt, string otherParameters, Action<string> toastAction)
        {
            var parameters = $"{prompt}\r\n\r\nNegative prompt: {negativePrompt}\r\n{otherParameters}";
            CopyToClipboardInternal(parameters, "all parameters", toastAction);
        }

        private static void CopyToClipboardInternal(string value, string itemName, Action<string> toastAction)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Clipboard.SetDataObject(value, true);
                });
                toastAction?.Invoke($"Copied {itemName} to clipboard");
            }
            catch (COMException ex) when (ex.ErrorCode == unchecked((int)0x800401D0)) // CLIPBRD_E_CANT_OPEN
            {
                toastAction?.Invoke($"Failed to copy {itemName} to clipboard. The clipboard is in use.");
            }
            catch (Exception ex)
            {
                toastAction?.Invoke($"An error occurred while copying {itemName}: {ex.Message}");
            }
        }
    }

}
