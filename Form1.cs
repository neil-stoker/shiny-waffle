﻿using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace shiney_waffle
{
    public partial class Form1 : Form
    {
        private Script luaScript;

        public Form1()
        {
            InitializeComponent();

            InitialiseMoonsharp();
        }

        private static int Mul(int a, int b)
        {
            return a * b;
        }

        private string MoonSharpFactorialSource()
        {
            string script = @"
                -- defines a factorial function
                function fact(n)
                  if(n==0) then
                    return 1
                  else
                    return n*fact(n-1)
                  end
                end";

            return script;
        }

        private double CallbackTest()
        {
            string scriptCode = MoonSharpFactorialSource();

            luaScript = new Script();

            luaScript.DoString(MoonSharpFactorialSource());

            luaScript.Globals["Mul"] = (Func<int,int,int>)Mul;
            DynValue res = luaScript.Call(luaScript.Globals["fact"], 4);
            return res.Number;
        }
        private void InitialiseMoonsharp()
        {
            //// Sets up the moonsharp environment
            //try
            //{
            //    luaScript = new Script();
            //    luaScript.Options.ScriptLoader = new EmbeddedResourcesScriptLoader();
            //    Script.DefaultOptions.ScriptLoader = new EmbeddedResourcesScriptLoader();
            //    ((ScriptLoaderBase)luaScript.Options.ScriptLoader).ModulePaths = new string[] { "Scripts/?.lua", "Scripts/?" };
            //    luaScript.LoadFile("Scripts/smwrapper.lua",null,"wrapper");
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"EXCEPTION {ex}");
            //    throw;
            //}
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //DynValue res = luaScript.Globals.Get(luaScript.Globals["getStates"]);
            label1.Text = "Result of function is " + CallbackTest().ToString();
        }
    }
}
