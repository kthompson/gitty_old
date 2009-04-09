using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gitty.Core;

namespace Gitty
{
    public abstract class Command
    {
        public Repository Repository { get; private set; }

        public abstract bool RequiresRepository { get; }
        public abstract bool RequiresArguments { get; }

        public Command(Repository repo)
        {
            this.Repository = repo;
        }

        /// <summary>
        /// Performs the action.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns>true is the action was handled</returns>
        public virtual bool PerformAction(params string[] args)
        {
            if (RequiresRepository && Repository == null)
            {
                ShowNoRepository();
                return true;
            }

            if (args.Length == 0 && this.RequiresArguments)
            {
                // we dont have any arguments but we should so let the user know.
                this.ShowUsage();
                return true;
            }
            else if (args.Length == 1)
            {
                switch (args.First())
                {
                    case "--help":
                    case "/?":
                        ShowHelp();
                        return true;
                }
            }
            
            return false;
        }

        public virtual void ShowUsage()
        {
        }

        public virtual void ShowHelp()
        {
        }

        public virtual void ShowNoRepository()
        {
            Console.WriteLine("fatal: Not a git repository (or any of the parent directories): .git");
        }
    }
}
