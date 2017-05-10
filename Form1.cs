﻿using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace shiney_waffle
{

    /// <summary>
    /// This is a form to test the LUA script handling.
    /// </summary>
    public partial class Form1 : Form
    {
        private static Script script;

        #region WinFormControls
        /// <summary>
        /// Main initialiser
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            createNewScript();
        }



        /// <summary>
        /// Creates a new script based on the standard template.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newScript_Click(object sender, EventArgs e)
        {
            createNewScript();
        }



        /// <summary>
        /// Load a script from the file system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openScript_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            
            openFileDialog.InitialDirectory= Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog.Filter = "lua files (*.lua)|*.lua|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;

            if(openFileDialog.ShowDialog()==DialogResult.OK)
            {
                if(File.Exists(openFileDialog.FileName))
                {
                    richTextBoxWithLineNumbers1.Text = File.ReadAllText(openFileDialog.FileName);
                }
            }
        }



        /// <summary>
        /// Save Script to file system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveScript_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();

            saveDialog.Filter = "lua files (*.lua)|*.lua|All files (*.*)|*.*";
            saveDialog.FilterIndex = 0;
            saveDialog.RestoreDirectory = true;
            saveDialog.OverwritePrompt = true;
            saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if(saveDialog.ShowDialog()==DialogResult.OK)
            {
                File.WriteAllText(saveDialog.FileName, richTextBoxWithLineNumbers1.Text);
            }
        }



        /// <summary>
        /// Run the test code.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void testScript_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Test run started");
            InitialiseLUAScripting();

            try
            {
                script.DoString(richTextBoxWithLineNumbers1.Text);

            }
            catch (ScriptRuntimeException ex)
            {
                Console.WriteLine($"RUNTIME Exception {ex.DecoratedMessage}");
                Application.Exit();
            }
            catch (SyntaxErrorException ex)
            {
                Console.WriteLine($"Script Syntax Error Exception {ex.DecoratedMessage}");
                Application.Exit();
            }
            catch (InvalidCastException ex)
            {
                Console.WriteLine($"Script Syntax Error Exception {ex.Message}\n{ex.StackTrace}");
                Application.Exit();
            }
        }
        #endregion



        /// <summary>
        /// Create a new script based on the template
        /// </summary>
        private void createNewScript()
        {
            richTextBoxWithLineNumbers1.Clear();

            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = "Scripts.client_sm.lua";

            using (Stream stream = assembly.GetManifestResourceStream("shiney_waffle." + resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                richTextBoxWithLineNumbers1.Text = reader.ReadToEnd();
            }
        }

        #region LUA_ScriptHandling

        public bool FSM_Fail(string message)
        {
            Console.WriteLine($"FAM_Fail said '{message}'");
            Console.Write($"FSM_Fail state entered message '{message}'");
            return true;
        }

        /// <summary>
        /// This is called by the state machine as a callback when a student has failed the module.
        /// </summary>
        /// \note The state machine will automatically move to the shutdown state
        public bool FSM_Pass(string message)
        {
            Console.Write($"FSM_Pass said '{message}'");
            Console.Write($"FSM_Pass state entered '{message}'");
            return true;
        }

        /// <summary>
        /// This is called by the state machine as a callback when shutting down.
        /// </summary>
        public bool FSM_Shutdown()
        {
            Console.Write("FSM_Shutdown state entered");

            Console.Write("And done...");
            return true;
        }



        /// <summary>
        /// 
        /// </summary>
        private void InitialiseLUAScripting()
        {
            Console.WriteLine("InitialiseLUAScripting()");
            try
            {
                script = new Script();
                script.Options.DebugPrint = s => { Console.WriteLine($"DEBUG LUA Script said '{s}'"); };
                script.Options.ScriptLoader = new EmbeddedResourcesScriptLoader();
                ((ScriptLoaderBase)script.Options.ScriptLoader).ModulePaths = ScriptLoaderBase.UnpackStringPaths("Scripts/?;Scripts/?.lua");
                script.Globals["FSM_Fail"] = (Func<string, bool>)FSM_Fail;
                script.Globals["FSM_Pass"] = (Func<string, bool>)FSM_Pass;
                script.Globals["FSM_Shutdown"] = (Func<bool>)FSM_Shutdown;

            }
            catch (ScriptRuntimeException ex)
            {
                Console.WriteLine($"RUNTIME Exception {ex.DecoratedMessage}");
                Application.Exit();
            }
            catch (SyntaxErrorException ex)
            {
                Console.WriteLine($"Script Syntax Error Exception {ex.DecoratedMessage}");
                Application.Exit();
            }
            catch (InvalidCastException ex)
            {
                Console.WriteLine($"Script Syntax Error Exception {ex.Message}\n{ex.StackTrace}");
                Application.Exit();
            }

            Console.WriteLine("InitialiseLUAScripting exiting");
        }


        #endregion
    }
}
