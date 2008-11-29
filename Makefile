DIRS = Gitty.Lib.CSharp Gitty Gitty.Lib.CommandLine

all:
	for a in $(DIRS); do (cd $$a; make) || exit 1; done

clean:
	for a in $(DIRS); do (cd $$a; echo "Making clean in $$a"; make clean); done
