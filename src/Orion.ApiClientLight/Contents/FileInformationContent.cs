using System.IO;

namespace Orion.ApiClientLight.Contents {
	internal class FileInformationContent {
		public Stream Stream { get; }
		public string Name { get; }

		public FileInformationContent(Stream file, string filename) {
			Name = filename;
			Stream = file;
		}

	}
}