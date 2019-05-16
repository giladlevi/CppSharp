using CppSharp.AST;
using CppSharp.Passes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CppSharp.Generators;
using CppSharp.Generators.CSharp;

namespace CppSharp
{
  class SDL : ILibrary
  {
    public void Setup(Driver driver)
    {
      var options = driver.Options;
      //options.CheckSymbols = true;
      options.IgnoreNotImplemenedCtors = true;
      options.GeneratorKind = GeneratorKind.CSharp;
      options.GenerateSequentialLayout = true;

      var lts = options.AddModule("LTSApi");
      lts.OutputNamespace = "LTSApi";
      lts.Headers.Add(@"C:\git\ChinaApi\ltsop_traderapi_20151230\includeWithoutComments\SecurityFtdcMdApi.h");
      lts.Headers.Add(@"C:\git\ChinaApi\ltsop_traderapi_20151230\includeWithoutComments\SecurityFtdcQueryApi.h");
      lts.Headers.Add(@"C:\git\ChinaApi\ltsop_traderapi_20151230\includeWithoutComments\SecurityFtdcTraderApi.h");
      lts.Headers.Add(@"C:\git\ChinaApi\ltsop_traderapi_20151230\includeWithoutComments\SecurityFtdcUserApiDataType.h");
      lts.Headers.Add(@"C:\git\ChinaApi\ltsop_traderapi_20151230\includeWithoutComments\SecurityFtdcUserApiStruct.h");
      //lts.SharedLibraryName = "securitymduserapi";
      lts.LibraryDirs.Add(@"C:\git\ChinaApi\ltsop_traderapi_20151230\libs\");
      lts.Libraries.Add(@"securitymduserapi.dll");
      lts.Libraries.Add(@"securityqueryapi.dll");
      lts.Libraries.Add(@"securitytraderapi.dll");

      //var xspeedMd = options.AddModule("XSpeedApi");
      //xspeedMd.OutputNamespace = "XSpeedApi";
      //xspeedMd.Headers.Add(@"C:\Git\CsGenerator\CsGenerator\DFITCApiDataType.h");
      //xspeedMd.Headers.Add(@"C:\Git\CsGenerator\CsGenerator\DFITCApiStruct.h");
      //xspeedMd.Headers.Add(@"C:\Git\CsGenerator\CsGenerator\DFITCMdApi.h");
      //xspeedMd.Headers.Add(@"C:\Git\CsGenerator\CsGenerator\DFITCTraderApi.h");

      //var xoneTrader = options.AddModule("XOneApi");
      //xoneTrader.OutputNamespace = "XOneApi";
      //xoneTrader.Headers.Add(@"C:\git\ChinaApi\XOne\XOneWithoutComments\X1FtdcApiDataType.h");
      //xoneTrader.Headers.Add(@"C:\git\ChinaApi\XOne\XOneWithoutComments\X1FtdcApiStruct.h");
      //xoneTrader.Headers.Add(@"C:\git\ChinaApi\XOne\XOneWithoutComments\X1FtdcTraderApi.h");
    }

    public void SetupPasses(Driver driver)
    {
    }

    public void Preprocess(Driver driver, ASTContext ctx)
    {
      var t = new Dictionary<string, List<string>>();
      foreach (var header in ctx.TranslationUnits)
      {
        if (header.FilePath == "<invalid>") continue;

        switch (header.FileName)
        {
          case "DFITCApiDataType.h":
            XSpeedGenerateEnumsFromMacros(ctx, header);
            break;
          case "X1FtdcApiDataType":
            break;
          case "SecurityFtdcUserApiDataType.h":
            LTSGenerateEnumsFromMacros(ctx, header);
            break;
          case "DFITCApiStruct.h":
          case "SecurityFtdcUserApiStruct.h":
          case "X1FtdcApiStruct.h":
            header.Declarations.ForEach(c => ctx.SetClassAsValueType(c.Name));
            break;
        }
      }
    }

    private static void LTSGenerateEnumsFromMacros(ASTContext ctx, TranslationUnit header)
    {
      var lines = File.ReadAllLines(header.FilePath);
      var reverseLines = lines.Reverse().ToArray();
      var line = reverseLines.GetEnumerator();
      line.MoveNext();

      var regex = new Regex(@"(SECURITY_FTDC_.*?)\s");

      var typedefs = header.Typedefs.Reverse().ToList();
      foreach (var typedef in typedefs)
      {
        var nativeType = typedef.QualifiedType.Type.ToNativeString();
        if (nativeType != "char") continue;

        while (line.Current.ToString().IndexOf(typedef.Name, StringComparison.Ordinal) < 0)
        {
          line.MoveNext();
        }
        line.MoveNext();
        while(line.Current.ToString() == "")
          line.MoveNext();
        var macros = new List<string>();
        while (line.Current.ToString().IndexOf("#define", StringComparison.Ordinal) == 0)
        {
          var a = regex.Match(line.Current.ToString()).Groups[1].Value;
          macros.Add(a);

          line.MoveNext();
        }
        if (macros.Count <= 0) continue;
        var b = ctx.GenerateEnumFromMacros(typedef.Name, macros.ToArray());
        b.Namespace = ctx.FindEnum("SECURITY_TE_RESUME_TYPE").ToList()[0].Namespace;
      }
    }

    private static void XSpeedGenerateEnumsFromMacros(ASTContext ctx, TranslationUnit header)
    {
      var lines = File.ReadAllLines(header.FilePath);
      var line = lines.GetEnumerator();
      line.MoveNext();

      var regex = new Regex(@"(DFITC_.*?)\s");

      var typedefs = header.Typedefs.ToList();
      foreach (var typedef in typedefs)
      {
        var nativeType = typedef.QualifiedType.Type.ToNativeString();
        if (nativeType != "short" && nativeType != "int") continue;

        while (line.Current.ToString().IndexOf(typedef.Name, StringComparison.Ordinal) < 0)
        {
          line.MoveNext();
        }
        line.MoveNext();
        var macros = new List<string>();
        while (line.Current.ToString().IndexOf("#define", StringComparison.Ordinal) == 0)
        {
          var a = regex.Match(line.Current.ToString()).Groups[1].Value;
          macros.Add(a);

          line.MoveNext();
        }
        if (macros.Count <= 0) continue;
        var b = ctx.GenerateEnumFromMacros(typedef.Name, macros.ToArray());
        b.Namespace = ctx.FindEnum("DFITC_TE_RESUME_TYPE").ToList()[0].Namespace;
      }
    }

    public void Postprocess(Driver driver, ASTContext ctx)
    {
      foreach (var m in driver.Context.Options.Modules)
      {
        var gen = driver.Generator;
        var gens = gen.Generate(m.Units.GetGenerated());
        foreach (CSharpSources g in gens)
        {
          var classes = g.TranslationUnits.SelectMany(u => u.Classes);
          foreach (var c in classes)
          {
            g.WriteLine("struct " + c.OriginalName);
            g.GenerateClassInternals(c);
          }

          var s = g.Generate().Replace("\r\npublic partial struct __Internal", "").Replace("[SuppressUnmanagedCodeSecurity]", "");
        }
      }
    }

    static class Program
    {
      public static void Main(string[] args)
      {
        ConsoleDriver.Run(new SDL());
      }
    }
  }
}
