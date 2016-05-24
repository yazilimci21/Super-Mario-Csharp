using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Core.Animation
{
    public class Animator : DisposableObject
    {
        #region winmm codes
        [Flags]
        public enum SoundFlags
        {
            /// <summary>play synchronously (default)</summary>
            SND_SYNC = 0x0000,
            /// <summary>play asynchronously</summary>
            SND_ASYNC = 0x0001,
            /// <summary>silence (!default) if sound not found</summary>
            SND_NODEFAULT = 0x0002,
            /// <summary>pszSound points to a memory file</summary>
            SND_MEMORY = 0x0004,
            /// <summary>loop the sound until next sndPlaySound</summary>
            SND_LOOP = 0x0008,
            /// <summary>don't stop any currently playing sound</summary>
            SND_NOSTOP = 0x0010,
            /// <summary>Stop Playing Wave</summary>
            SND_PURGE = 0x40,
            /// <summary>The pszSound parameter is an application-specific alias in the registry. You can combine this flag with the SND_ALIAS or SND_ALIAS_ID flag to specify an application-defined sound alias.</summary>
            SND_APPLICATION = 0x80,
            /// <summary>don't wait if the driver is busy</summary>
            SND_NOWAIT = 0x00002000,
            /// <summary>name is a registry alias</summary>
            SND_ALIAS = 0x00010000,
            /// <summary>alias is a predefined id</summary>
            SND_ALIAS_ID = 0x00110000,
            /// <summary>name is file name</summary>
            SND_FILENAME = 0x00020000,
            /// <summary>name is resource name or atom</summary>
            SND_RESOURCE = 0x00040004
        }

        [DllImport("winmm.dll", SetLastError = true)]
        static extern bool PlaySound(string pszSound, UIntPtr hmod, uint fdwSound);

        [DllImport("winmm.dll", SetLastError = true)]
        static extern bool PlaySound(byte[] pszSound, IntPtr hmod, SoundFlags fdwSound);

        #endregion
        public List<Animation> Animates = new List<Animation>();
        public Animation currentAnimation = new Animation();
        public int currentAnimIndex = 0, duractionTime = 80;
        public bool Loop = true, isStop = false;
        public event MethodInvoker isFinish, RePlay;

        public override void Disposed(bool dispose)
        {
            if (dispose)
            {
                for (int i = 0; i < Animates.Count; i++)
                {
                    Animates[i].Dispose();
                }
                GC.SuppressFinalize(currentAnimation);
            }
            base.Disposed(dispose);
        }

        private void OnFinish()
        {
            isStop = true;
            if(isFinish != null)
            {
                isFinish();
            }
        }

        private void OnRePlay()
        {
            if (RePlay != null)
            {
                RePlay();
            }
        }

        #region Loads
        public Animator()
        {
            
        }

        public Animator(Animation Anim) : this()
        {
            AddFrame(Anim);
        }

        public Animator(Image image, string sound)
            : this()
        {
            AddFrame(image, sound, new SizeF(image.Width, image.Height), duractionTime);
        }

        public Animator(Image image, string sound, SizeF imageRect)
            : this()
        {
            AddFrame(image, sound, imageRect, duractionTime);
        }

        public Animator(Image image, string sound, SizeF imageRect, bool isPauseAnim)
            : this()
        {
            AddFrame(image, sound, imageRect, duractionTime, isPauseAnim);
        }

        public Animator(Image image, string sound, SizeF imageRect, int duractionTime)
            : this()
        {
            AddFrame(image, sound, imageRect, duractionTime, false);
        }
        #endregion

        #region Properties
        public PointF Center
        {
            get
            {
                return new PointF(currentAnimation.AnimSize.Width / 2, currentAnimation.AnimSize.Height / 2);
            }
        }

        public SizeF AnimSize
        {
            get
            {
                return currentAnimation.AnimSize;
            }
        }

        public void FlipY()
        {
            for (int i = 0; i < Animates.Count; i++)
            {
                RET:
                try
                {
                    Animates[i].Anim.RotateFlip(RotateFlipType.Rotate180FlipY);
                }
                catch
                {
                    Thread.Sleep(10);
                    goto RET;
                }
            }
        }

        public void FlipX()
        {
            for (int i = 0; i < Animates.Count; i++)
            {
            RET:
                try
                {
                    Animates[i].Anim.RotateFlip(RotateFlipType.Rotate180FlipX);
                }
                catch
                {
                    Thread.Sleep(10);
                    goto RET;
                }
            }
        }

        #endregion

        #region AddFrame voids
        public void AddFrame(Animation Anim)
        {
            if(Anim != default(Animation))
            {
                Animates.Add(Anim);
            }
        }

        public void AddFrame(Image image, string sound)
        {
            AddFrame(image, sound, new SizeF(image.Width, image.Height), duractionTime);
        }

        public void AddFrame(Image image, string sound, SizeF imageRect)
        {
            AddFrame(image, sound, imageRect, duractionTime);
        }

        public void AddFrame(Image image, string sound, SizeF imageRect, bool isPauseAnim)
        {
            AddFrame(image, sound, imageRect, duractionTime, isPauseAnim);
        }

        public void AddFrame(Image image, string sound, SizeF imageRect, int duractionTime)
        {
            AddFrame(image, sound, imageRect, duractionTime, false);
        }

        public void AddFrame(Image image, string sound, SizeF imageRect, int duractionTime, bool isPauseAnim)
        {
            if (image != null)
            {
                if (ImageAnimator.CanAnimate(image))
                {
                    int imgCount = image.GetFrameCount(System.Drawing.Imaging.FrameDimension.Time);
                    for (int i = 0; i < imgCount; i++)
                    {
                        image.SelectActiveFrame(System.Drawing.Imaging.FrameDimension.Time, i);
                        Animation anim = new Animation();
                        anim.Anim = (Image)image.Clone();
                        anim.sound = sound;
                        anim.AnimSize = imageRect;
                        anim.duractionTime = duractionTime;
                        anim.isPauseAnim = isPauseAnim;
                        Animates.Add(anim);
                    }
                    image.Dispose();
                }
                else
                {
                    Animation anim = new Animation();
                    anim.Anim = (Image)image.Clone();
                    anim.sound = sound;
                    anim.AnimSize = imageRect;
                    anim.duractionTime = duractionTime;
                    anim.isPauseAnim = isPauseAnim;
                    Animates.Add(anim);
                    image.Dispose();
                }
                image.Dispose();
                currentAnimation = Animates[0];
                currentAnimIndex = 0;
            }
        }

        #endregion

        #region Animation Player
        public void Play()
        {
            gotoAndPlay(0);
        }

        public void Stop()
        {
            gotoAndStop(0);
        }

        public void gotoAndPlay(int animIndex)
        {
            if (Animates.Count > 0 && animIndex < Animates.Count && animIndex >= 0)
            {
                currentAnimation = Animates[animIndex];
                currentAnimIndex = animIndex;
                if (!currentAnimation.isPauseAnim)
                {
                    isStop = false;
                }
            }
        }

        public void gotoAndStop(int animIndex)
        {
            if (Animates.Count > 0 && animIndex < Animates.Count && animIndex >= 0)
            {
                currentAnimation = Animates[animIndex];
                currentAnimIndex = animIndex;
            }
        }

        private float animtime = 0;

        public void AnimTick(float time)
        {
            if (Animates.Count > 1 && !isStop)
            {
                currentAnimation = Animates[currentAnimIndex];
                if (!string.IsNullOrEmpty(currentAnimation.sound))
                {
                    Play(currentAnimation.sound);
                }
                if (!currentAnimation.isPauseAnim)
                {
                    animtime += time;
                    if (animtime >= currentAnimation.duractionTime)
                    {
                        currentAnimIndex++;
                        animtime = 0;
                        if (currentAnimIndex >= Animates.Count)
                        {
                            if (this.Loop)
                            {
                                OnRePlay();
                                currentAnimIndex = 0;
                            }
                            else
                            {
                                OnFinish();
                            }
                        }
                    }
                }
            }
        }

        private void Play(string file)
        {
            if (File.Exists(file))
            {
                PlaySound(file, UIntPtr.Zero, (uint)(SoundFlags.SND_FILENAME | SoundFlags.SND_ASYNC));
            }
        }

        private void Play(byte[] waveData)
        {
            if (waveData.Length > 0)
            {
                PlaySound(waveData, IntPtr.Zero, SoundFlags.SND_ASYNC | SoundFlags.SND_MEMORY);
            }
        }

        #endregion
    }
}
