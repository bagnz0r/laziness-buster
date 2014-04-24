using System;
using System.IO;
using System.Diagnostics;

namespace lazinessbuster
{
	class MainClass
	{
		/// <summary>
		/// The current dir.
		/// </summary>
		static string currentDir;

		/// <summary>
		/// The threshold of changes at which commits should be made.
		/// </summary>
		static int threshold = 10;

		/// <summary>
		/// The current change count.
		/// </summary>
		static int currentChangeCount = 0;

		public static void Main (string[] args)
		{
			Console.WriteLine ("Laziness Buster v1.0");
			Console.WriteLine ("(c) 2014 by bagnz0r <http://github.com/bagnz0r>");

			// Look for .git in the current path, and create new repo if one does not exist.
			currentDir = Environment.CurrentDirectory;
			if (!Directory.Exists (currentDir + "\\.git")) {
				Run ("git", "init");
			}

			// Create a watcher.
			FileSystemWatcher watcher = new FileSystemWatcher ();
			watcher.Path = currentDir;
			watcher.IncludeSubdirectories = true;
			watcher.Filter = "*";
			watcher.NotifyFilter = NotifyFilters.LastWrite |
				NotifyFilters.Attributes |
				NotifyFilters.DirectoryName |
				NotifyFilters.FileName |
				NotifyFilters.Size;
			watcher.Changed += new FileSystemEventHandler(OnChanged);
			watcher.Created += new FileSystemEventHandler(OnChanged);
			watcher.Deleted += new FileSystemEventHandler(OnChanged);
			watcher.Renamed += new RenamedEventHandler(OnRenamed);
			watcher.EnableRaisingEvents = true;

			// Don't you dare ookin' die.
			while (true) {
			}
		}

		/// <summary>
		/// Raises the changed event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Arguments.</param>
		static void OnChanged(object sender, FileSystemEventArgs args)
		{
			Console.WriteLine ("File '" + args.Name + "' has changed");
			currentChangeCount++;

			if (currentChangeCount >= threshold) {
				Commit ();
				currentChangeCount = 0;
			}
		}

		/// <summary>
		/// Raises the renamed event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Arguments.</param>
		static void OnRenamed(object sender, RenamedEventArgs args)
		{
			Console.WriteLine ("File '" + args.OldName + "' has been renamed to '" + args.Name + "'");
			currentChangeCount++;

			if (currentChangeCount >= threshold) {
				Commit ();
				currentChangeCount = 0;
			}
		}

		/// <summary>
		/// Commit, obviously.
		/// </summary>
		static void Commit()
		{
			Console.WriteLine ("Commiting changes now!");
			Run ("git", "add --all .");
			Run ("git", "commit -m \"" + new DateTime ().ToShortDateString () 
				+ " " + new DateTime ().ToShortTimeString () + "\"");
		}

		/// <summary>
		/// Run the specified command with arg.
		/// </summary>
		/// <param name="command">Command.</param>
		/// <param name="arg">Argument.</param>
		static void Run (string command, string arg)
		{
			Process p = new Process ();
			p.StartInfo.FileName = command;
			p.StartInfo.Arguments = arg;
			p.Start ();
			p.WaitForExit ();
		}

	}
}
