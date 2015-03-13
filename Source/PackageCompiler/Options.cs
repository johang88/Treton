using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageCompiler
{
	class Options
	{
		[Option('i', "input", Required = true, HelpText = "Content location")]
		public string InputPath { get; set; }
		[Option('o', "output", Required = true, HelpText = "Game data directory, \"_platform\" will be concatenated")]
		public string OutputPath { get; set; }
		[Option('c', "packages", Required = true, HelpText = "Packages to compile (comma separated)")]
		public string Packages { get; set; }
		[Option('p', "platform", Required = true, HelpText = "Platform to compile")]
		public string Platforms { get; set; }
		[Option('b', "bundle", Required = false, HelpText = "If true then all resources in a package will be bundled with the package description")]
		public bool CreateBundle { get; set; }

		[HelpOption]
		public string GetUsage()
		{
			return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
		}
	}
}
