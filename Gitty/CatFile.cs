using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gitty.Core;

namespace Gitty
{
    public class CatFile : Command
    {
        public override bool RequiresRepository
        {
            get { return true; }
        }

        public override bool RequiresArguments
        {
            get { return true; }
        }

        public CatFile(Repository repo)
            : base(repo)
        {
        }

        public override bool PerformAction(params string[] args)
        {
            if (base.PerformAction(args)) return true;

            if (args.Length != 2)
            {
                ShowUsage();
                return true;
            }

            //TODO: This should support rev-parsing 
            var id = ObjectId.FromString(args[1]);


            switch (args[0])
            {
                case "-t":
                    var ot = this.Repository.OpenObject(id);
                    Console.WriteLine(ot.ObjectType.ToString());
                    break;
                case "-s":
                    var os = this.Repository.OpenObject(id);
                    Console.WriteLine(os.Size.ToString());
                    break;
                case "-p":
                    var op = this.Repository.OpenObject(id);
                    var content = Encoding.ASCII.GetString(op.Bytes);
                    Console.WriteLine(content);
                    break;
                case "tree":
                    var o = this.Repository.OpenTree(id);
                    var treeContent = Encoding.ASCII.GetString(o.Bytes);
                    Console.WriteLine(treeContent);
                    break;
                case "blob":
                    var blob = this.Repository.OpenBlob(id);
                    var blobContent = Encoding.ASCII.GetString(blob.Bytes);
                    Console.WriteLine(blobContent);
                    break;
                case "commit":
                    var commit = this.Repository.MapCommit(args[1]);
#warning this needs a bit of revision
                    Console.WriteLine(commit.Message);
                    break;
                case "tag":
#warning this needs a bit of revision too
                    var tag = this.Repository.MapTag(args[1], id);
                    Console.WriteLine(tag.TagName);
                    break;
                default:
                    break;
            }

            return false;
        }

        public override void ShowUsage()
        {
            Console.WriteLine("usage: gitty cat-file [-t|-s|-p|<type>] <sha1>");
            Console.WriteLine();
            Console.WriteLine("<type> can be one of: blob, tree, commit, tag");
            Console.WriteLine("    -t                    show object type");
            Console.WriteLine("    -s                    show object size");
            Console.WriteLine("    -p                    pretty-print object's content");
        }
    }
}
