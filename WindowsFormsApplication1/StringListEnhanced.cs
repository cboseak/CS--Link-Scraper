﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    class StringListEnhanced : List<string>
    {
        public void RemoveDuplicate()
        {
            List<string> temp = this.Distinct().ToList();

            this.Clear();
            foreach (var i in temp)
            {
                this.Add(i);
            }
        }
        public async void RemoveDuplicate(bool runAsync)
        {
            if (!runAsync)
                RemoveDuplicate();
            else if (runAsync)
                await RemoveDuplicateAsync();
        }
        internal Task RemoveDuplicateAsync()
        {
            return Task.Run(() => { 
                List<string> temp = new List<string>();
                temp = this.Distinct().ToList();
                this.Clear();
                Parallel.ForEach(temp, i => {
                    this.Add(i);
                });
            });

        }

        public void RemoveEmpty()
        {
            Regex whiteSpace = new Regex(@"/^\s*$/");
            Regex emptyString = new Regex(@"^$");
            List<string> temp = new List<string>();

            foreach (var i in this)
            {
                if (!emptyString.IsMatch(i) && !whiteSpace.IsMatch(i) && !string.IsNullOrEmpty(i) && !string.IsNullOrWhiteSpace(i))
                {
                    temp.Add(i);
                }
            }
            this.Clear();
            foreach (var i in temp)
            {
                this.Add(i);
            }
        }

        public async void RemoveEmpty(bool runAsync)
        {
            if (!runAsync)
                RemoveEmpty();
            else if (runAsync)
                await RemoveEmptyAync();
        }

        public void Pop()
        {
            this.RemoveAt(0);
        }

        public StringListEnhanced HCollectionToCL(HtmlElementCollection collectionIn)
        {
            StringListEnhanced temp = new StringListEnhanced();
            foreach (HtmlElement i in collectionIn)
            {
                temp.Add(i.ToString());
            }
            return temp;
        }
        public void HCollectionToCL(HtmlElementCollection collectionIn, bool runAsync)
        {
            if (runAsync)
                HCollectionToCLAsync(collectionIn);
            else if (!runAsync)
                HCollectionToCL(collectionIn);
        }

        internal StringListEnhanced HCollectionToCLAsync(HtmlElementCollection collectionIn)
        {
            StringListEnhanced temp = new StringListEnhanced();
            foreach (HtmlElement i in collectionIn)
            {
                temp.Add(i.ToString());
            }
            return temp;
        }

        public void RemoveIfRegex(Regex regToCheck)
        {
            StringListEnhanced temp = new StringListEnhanced();
            foreach (var i in this)
            {
                if (!regToCheck.IsMatch(i))
                {
                    temp.Add(i);
                }
            }
            this.Clear();
            foreach (var i in temp)
            {
                this.Add(i);
            }
        }

        public void RemoveIfNotRegex(Regex regToCheck)
        {
            StringListEnhanced temp = new StringListEnhanced();
            foreach (var i in this)
            {
                if (regToCheck.IsMatch(i))
                {
                    temp.Add(i);
                }
            }
            this.Clear();
            foreach (var i in temp)
            {
                this.Add(i);
            }
        }

        public int NumberOfOccurances(string stringIn)
        {
            int counter = 0;
            foreach (var i in this)
            {
                if (i == stringIn)
                    counter++;
            }
            return counter;
        }

        public void Concatenate(StringListEnhanced listIn)
        {
            this.AddRange(listIn);
        }

        internal Task RemoveEmptyAync()
        {
            return Task.Run(() => { 
                Regex whiteSpace = new Regex(@"/^\s*$/");
                Regex emptyString = new Regex(@"^$");
                List<string> temp = new List<string>();

                Parallel.ForEach(this, i => {
                    if (!emptyString.IsMatch(i) && !whiteSpace.IsMatch(i) && !string.IsNullOrEmpty(i) && !string.IsNullOrWhiteSpace(i))
                    {
                        temp.Add(i);
                    }
                });

                this.Clear();

                Parallel.ForEach(temp, i => {
                    this.Add(i);
                });
            });
        }


    }
}
