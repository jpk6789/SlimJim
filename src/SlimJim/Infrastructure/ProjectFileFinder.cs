﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using log4net;
using SlimJim.Model;

namespace SlimJim.Infrastructure
{
	public class ProjectFileFinder
	{
		private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private readonly List<Regex> ignorePatterns;

		private string[] _supportedExtensions = ProjectFileType.AllExtentions.Select(c => c.FileExtention).ToArray();


		public ProjectFileFinder()
		{
			ignorePatterns = new List<Regex>();
			IgnorePatterns(@"^\.svn$", @"^\.hg$", @"^\.git$", "^bin$", "^obj$", "ReSharper");
		}

		public void SetProjectTypes(ProjectFileType[] projectTypes)
		{
			if (projectTypes?.Length > 0)
			{
				_supportedExtensions = projectTypes.Select(c => c.FileExtention).ToArray();
				Log.InfoFormat($"The following project types are selected: '{string.Join(", ", projectTypes.Select(c => c.Name).ToArray())}'.");
			}
			else
			{
				Log.WarnFormat($"No valid project types selected.");
			}
		}

		public virtual List<FileInfo> FindAllProjectFiles(string startPath)
		{
			Log.InfoFormat($"Searching for '{string.Join(", ", _supportedExtensions)}' files at '{startPath}'.");

			var root = new DirectoryInfo(startPath);
			var projectFiles = new List<FileInfo>();
			if (root.Exists)
				projectFiles = GetProjectFiles(root);
			else
				Log.WarnFormat($"Defined path '{root}' does not exist.");

			return projectFiles;
		}

		private List<FileInfo> GetProjectFiles(DirectoryInfo directory)
		{
			var files = new List<FileInfo>();

			if (!PathIsIgnored(directory.Name))
				SearchDirectoryForProjects(directory, files);
			else
				Log.WarnFormat($"Directory '{directory.Name}' is skipped due to filter rules.");

			return files;
		}

		private void SearchDirectoryForProjects(DirectoryInfo directory, List<FileInfo> files)
		{
			FileInfo[] projects = directory
									.GetFiles("*.*", SearchOption.AllDirectories)
									.Where(f => _supportedExtensions.Contains(f.Extension, StringComparer.OrdinalIgnoreCase))
									.Where(f => !PathIsIgnored(f.Name))
									.ToArray();
			if (projects.Length > 0)
			{
				AddProjectFile(projects, files);
			}
			else
			{
				RecurseChildDirectories(directory, files);
			}
		}

		private void RecurseChildDirectories(DirectoryInfo directory, List<FileInfo> files)
		{
			foreach (DirectoryInfo dir in directory.EnumerateDirectories())
			{
				files.AddRange(GetProjectFiles(dir));
			}
		}

		private void AddProjectFile(IEnumerable<FileInfo> projects, List<FileInfo> files)
		{
			foreach (var project in projects)
			{
				files.Add(project);
				Log.Debug(project);
			}
		}

		public bool PathIsIgnored(DirectoryInfo dir)
		{
			return PathIsIgnored(dir.Name);
		}

		public bool PathIsIgnored(string name)
		{
			return ignorePatterns.Exists(p => p.IsMatch(name));
		}

		public virtual void IgnorePatterns(params string[] patterns)
		{
			var regexes = new List<string>(patterns).ConvertAll(p => new Regex(p, RegexOptions.IgnoreCase));
			ignorePatterns.AddRange(regexes);
		}
	}
}
