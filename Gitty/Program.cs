using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gitty.Core;

namespace Gitty
{
    class Program
    {
        static void Main(string[] args)
        {
            
            if (args.Length > 0)
            {
                Repository repo = Repository.Open(".");
                Command cmd;
                switch (args[0])
                {
                    case "cat-file":
                    //cmd = new CatFile(Repository.Find()
                    //case "add--interactive
                    case "add":
                    case "am":
                    case "annotate":
                    case "apply":
                    case "archimport":
                    case "archive":
                    case "bisect":
                    case "blame":
                    case "branch":
                    case "bundle":
                    case "check-attr":
                    case "check-ref-format":
                    case "checkout-index":
                    case "checkout":
                    case "cherry-pick":
                    case "cherry":
                    case "citool":
                    case "clean":
                    case "clone":
                    case "commit-tree":
                    case "commit":
                    case "config":
                    case "count-objects":
                    case "cvsexportcommit":
                    case "cvsimport":
                    case "cvsserver":
                    case "daemon":
                    case "describe":
                    case "diff-files":
                    case "diff-index":
                    case "diff-tree":
                    case "diff":
                    case "fast-export":
                    case "fast-import":
                    case "fetch--tool":
                    case "fetch-pack":
                    case "fetch":
                    case "filter-branch":
                    case "fmt-merge-msg":
                    case "for-each-ref":
                    case "format-patch":
                    case "fsck-objects":
                    case "fsck":
                    case "gc":
                    case "get-tar-commit-id":
                    case "grep":
                    case "gui":
                    //case "gui--askpass":
                    case "hash-object":
                    case "help":
                    case "http-fetch":
                    case "http-push":
                    case "imap-send":
                    case "index-pack":
                    case "init-db":
                    case "init":
                    case "instaweb":
                    case "log":
                    case "lost-found":
                    case "ls-files":
                    case "ls-remote":
                    case "ls-tree":
                    case "mailinfo":
                    case "mailsplit":
                    case "merge-base":
                    case "merge-file":
                    case "merge-index":
                    case "merge-octopus":
                    case "merge-one-file":
                    case "merge-ours":
                    case "merge-recursive":
                    case "merge-resolve":
                    case "merge-subtree":
                    case "merge-tree":
                    case "merge":
                    case "mergetool":
                    case "mktag":
                    case "mktree":
                    case "mv":
                    case "name-rev":
                    case "pack-objects":
                    case "pack-redundant":
                    case "pack-refs":
                    case "parse-remote":
                    case "patch-id":
                    case "peek-remote":
                    case "prune-packed":
                    case "prune":
                    case "pull":
                    case "push":
                    case "quiltimport":
                    case "read-tree":
                    case "rebase":
                    case "rebase--interactive":
                    case "receive-pack":
                    case "reflog":
                    case "relink":
                    case "remote":
                    case "repack":
                    case "repo-config":
                    case "request-pull":
                    case "rerere":
                    case "reset":
                    case "rev-list":
                    case "rev-parse":
                    case "revert":
                    case "rm":
                    case "send-email":
                    case "send-pack":
                    case "sh-setup":
                    case "shell":
                    case "shortlog":
                    case "show-branch":
                    case "show-index":
                    case "show-ref":
                    case "show":
                    case "stage":
                    case "stash":
                    case "status":
                    case "stripspace":
                    case "submodule":
                    case "svn":
                    case "symbolic-ref":
                    case "tag":
                    case "tar-tree":
                    case "unpack-file":
                    case "unpack-objects":
                    case "update-index":
                    case "update-ref":
                    case "update-server-info":
                    case "upload-archive":
                    case "upload-pack":
                    case "var":
                    case "verify-pack":
                    case "verify-tag":
                    //case "web--browse":
                    case "whatchanged":
                    case "write-tree":
                        Console.WriteLine("gitty: '{0}' is not currently supported by gitty.", args[0]);
                        break;
                    default:
                        Console.WriteLine("gitty: '{0}' is not a git-command. See 'gitty --help'.", args[0]);
                        break;
                }
            }
        }
    }
}
