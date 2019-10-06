using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HrUpdater
{
    public class StringArgs
    {
        public bool IsSkipBlank { get; set; } = true;
        public Dictionary<string, StringArgsValue> ArgumentList { get; set; } = new Dictionary<string, StringArgsValue>();

        public StringArgs()
        {
        }

        public StringArgsValue GetValue(string command)
        {
            if (ArgumentList.ContainsKey(command.ToLower()))
                return ArgumentList[command.ToLower()];
            else
                return new StringArgsValue();
        }

        public void AddArgument(string command, string description)
        {
            if (!ArgumentList.ContainsKey(command.ToLower()))
            {
                ArgumentList.Add(command.ToLower(), new StringArgsValue(description));
            }
        }

        public void LoadArgs(params string[] args)
        {
            string currentCommand = "";
            bool islaststringCommand = false;
            foreach (string s in args)
            {
                if (!(IsSkipBlank && s == ""))
                {
                    if (s.Length > 0 && s.Substring(0, 1) == "-") //command
                    {
                        if (islaststringCommand)
                            ArgumentList[currentCommand].Value = "";
                        currentCommand = s.Substring(1);
                        AddArgument(currentCommand, "");
                        islaststringCommand = true;
                    }
                    else if (currentCommand != "")
                    {
                        ArgumentList[currentCommand].Value = s;
                        islaststringCommand = false;
                    }
                }
            }
            if (islaststringCommand)
                ArgumentList[currentCommand].Value = "";
        }

        public string GetHelp()
        {
            StringBuilder sb = new StringBuilder();
            int maxcommandLength = 0;
            foreach (string key in ArgumentList.Keys)
            {
                if (key.Length > maxcommandLength)
                    maxcommandLength = key.Length;
            }
            foreach (KeyValuePair<string, StringArgsValue> arg in ArgumentList)
            {
                string[] strs = arg.Value.Description.Split('\n');
                bool isfirst = true;
                foreach (string s in strs)
                {
                    if (isfirst)
                    {
                        sb.AppendLine(arg.Key.PadRight(maxcommandLength, ' ') + " : " + s);
                        isfirst = false;
                    }
                    else
                    {
                        sb.AppendLine("".PadRight(maxcommandLength + 3, ' ') + s);
                    }
                }
            }
            return sb.ToString();
        }
    }

    public class StringArgsValue
    {
        public bool IsSet { private set; get; } = false;
        public string Description { get; set; } = "";
        public string LastArgValue { private set; get; } = "";

        private string fullValue = "";

        public string Value
        {
            get
            {
                return fullValue.Trim();
            }
            set
            {
                Values.Add(value);
                LastArgValue = value;
                fullValue += value + " ";
                IsSet = true;
            }
        }

        public List<string> Values { get; set; } = new List<string>();

        public StringArgsValue()
        {
        }

        public StringArgsValue(string description)
        {
            Description = description;
        }

        public StringArgsValue(string description, string value)
        {
            Description = description;
            Value = value;
        }
    }
}
