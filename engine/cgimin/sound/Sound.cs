using System;
using System.IO;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK;

namespace Engine.cgimin.sound
{
	public class Sound
	{
		private static AudioContext context;
        private int _source;
        public static int counter = 0;
		/// <summary>
		/// Zugriff auf den Index der Soundquelle.
		/// Dies wird als Ziel aller nativen OpenAL genutzt. 
		/// </summary>

		/// <summary>
		/// Aktueller Status des Soundeffektes.
		/// </summary>
		public ALSourceState State => AL.GetSourceState(_source);

		/// <summary>
		/// Gain des Soundeffektes.
		/// Erlaubt Werte zwischen 0f und 1f.
		/// </summary>
		public float Gain
		{
			get
			{
				float value;
				AL.GetSource(_source, ALSourcef.Gain, out value);
				return value;
			}
			set
			{
				AL.Source(_source, ALSourcef.Gain, value);
			}
		}

		/// <summary>
		/// Pitch des Soundeffektes.
		/// </summary>
		public float Pitch
		{
			get
			{
				float value;
				AL.GetSource(_source, ALSourcef.Pitch, out value);
				return value;
			}
			set
			{
				AL.Source(_source, ALSourcef.Pitch, value);
			}
		}

		/// <summary>
		/// Erstellt eine neue Instanz.
		/// </summary>
		/// <param name="filePath">Pfad zur .wav Datei.</param>
		/// <param name="looping">Entschiedet ob der Sound immer wiederholt werden soll.</param>
		public Sound(int buffer, bool looping = false)
		{

            _source = AL.GenSource();

			AL.Source(_source, ALSourceb.Looping, looping);

			AL.Source(_source, ALSourcei.Buffer, buffer);

            AL.Source(_source, ALSourcef.RolloffFactor, 1.5f);
            AL.Source(_source, ALSourcef.ReferenceDistance, 6);
            AL.Source(_source, ALSourcef.MaxDistance, 50);

		}

        public static int LoadSound(String filepath)
        {
            int buffer = AL.GenBuffer();
            int channels, bits_per_sample, sample_rate;
            byte[] sound_data = LoadWave(File.Open(filepath, FileMode.Open), out channels, out bits_per_sample, out sample_rate);
            AL.BufferData(buffer, GetSoundFormat(channels, bits_per_sample), sound_data, sound_data.Length, sample_rate);

            return buffer;
        }

		public void UnLoad()
		{
			AL.SourceStop(_source);
			AL.DeleteSource(_source);
		}

		/// <summary>
		/// Spielt den Sound ab.
		/// Wird der Sound bereits abgespielt startet er von vorne.
		/// </summary>
		public void Play()
		{
			AL.SourcePlay(_source);
		}

		/// <summary>
		/// Stoppt den Sound.
		/// </summary>
		public void Stop()
		{
			AL.SourceStop(_source);

		}

        public bool IsPlaying()
        {
            ALSourceState state = AL.GetSourceState(_source);
            return state == ALSourceState.Playing;
        }

		/// <summary>
		/// Pausiert den Sound an der aktuellen Position.
		/// </summary>
		public void Pause()
		{
			AL.SourcePause(_source);
		}

        public void SetPosition(Vector3 position)
        {
            AL.Source(_source, ALSource3f.Position, ref position);
        }


		/// <summary>
		/// Initialisiert die Sound Klasse.
		/// Muss vor dem Erstellen einer Sound Instanz aufgerufen werden.
		/// </summary>
		public static void Init()
		{
			context = new AudioContext();
            AL.DistanceModel(ALDistanceModel.LinearDistance);
            
		}

        public static void SetListener(Vector3 position, Vector3 velocity)
        {
            AL.Listener(ALListener3f.Position, ref position);
            AL.Listener(ALListener3f.Velocity, ref velocity);
        }

        public static void SetListenerOrientation(Vector3 position, Vector3 up)
        {
            AL.Listener(ALListenerfv.Orientation, ref position, ref up);
        }

        // Loads a wave/riff audio file.
        private static byte[] LoadWave(Stream stream, out int channels, out int bits, out int rate)
		{
			if (stream == null)
				throw new ArgumentNullException(nameof(stream));

			using (BinaryReader reader = new BinaryReader(stream))
			{
				// RIFF header
				string signature = new string(reader.ReadChars(4));
				if (signature != "RIFF")
					throw new NotSupportedException("Specified stream is not a wave file.");

				int riff_chunck_size = reader.ReadInt32();

				string format = new string(reader.ReadChars(4));
				if (format != "WAVE")
					throw new NotSupportedException("Specified stream is not a wave file.");

				// WAVE header
				string format_signature = new string(reader.ReadChars(4));
				if (format_signature != "fmt ")
					throw new NotSupportedException("Specified wave file is not supported.");

				int format_chunk_size = reader.ReadInt32();
				int audio_format = reader.ReadInt16();
				int num_channels = reader.ReadInt16();
				int sample_rate = reader.ReadInt32();
				int byte_rate = reader.ReadInt32();
				int block_align = reader.ReadInt16();
				int bits_per_sample = reader.ReadInt16();

				string data_signature = new string(reader.ReadChars(4));
				if (data_signature != "data")
					throw new NotSupportedException("Specified wave file is not supported.");

				int data_chunk_size = reader.ReadInt32();

				channels = num_channels;
				bits = bits_per_sample;
				rate = sample_rate;

				return reader.ReadBytes(data_chunk_size);
			}
		}

  


        private static ALFormat GetSoundFormat(int channels, int bits)
		{
			switch (channels)
			{
			case 1: return bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
			case 2: return bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
			default: throw new NotSupportedException("The specified sound format is not supported.");
			}
		}

        
	}
}

