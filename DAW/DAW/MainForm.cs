using System;
using System.IO;
using DAW.UI;
using Eto.Forms;
using Eto.Drawing;
using Ion.Audion;

namespace DAW
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            var engine = new AudioEngine();
            var audio = new Audio(1024, 44100);
            audio.SetEngine(engine);

            Title = "DAW";
            
            MinimumSize = new Size(400, 350);
            ClientSize = new Size(800, 600);
            
            WindowState = WindowState.Maximized;
            
            var button = new Button
            {
                Text = "Settings"
            };
            
            button.Click += (sender, e) =>
            {
                var settingsWindow = new SettingsWindow();
                settingsWindow.ShowModal(this);
            };
            
            var loadFileButton = new Button
            {
                Text = "Load File"
            };
            
            loadFileButton.Click += (sender, e) =>
            {
                var dialog = new OpenFileDialog();
                dialog.Filters.Add(new FileFilter("Audio Files", new string[] { ".wav", ".mp3" }));
                dialog.ShowDialog(this);

                if (dialog.FileName != null)
                {
                    var audioFile = new AudioFile(dialog.FileName);
                    var channel = new AudioChannel(Path.GetFileNameWithoutExtension(dialog.FileName));
                    channel.AddEvent(AudioEvent.CreateFromAudioFile(audioFile, 0));
                    engine.AddChannel(channel);
                }
            };
            
            var togglePlayButton = new Button
            {
                Text = "Toggle Play"
            };
            
            togglePlayButton.Click += (sender, e) =>
            {
                engine.TogglePlayback();
            };
            
            var resetButton = new Button
            {
                Text = "Reset"
            };
            
            resetButton.Click += (sender, e) =>
            {
                engine.Reset();
            };
            
            var channelRack = new ChannelRack(engine);
            
            Content = new StackLayout
            {
                Items =
                {
                    button,
                    loadFileButton,
                    togglePlayButton,
                    resetButton,
                    channelRack,
                }
            };
            
            
        }
    }
}