CSC=gmcs

SOURCES = \
	DiffFilesFile.cs	\
	DiffIndexFile.cs	\
	GitCommandLine.cs	\
	GitLib.cs		\
	GitPath.cs		\
	Index.cs		\
	LsFilesFile.cs		\
	Repository.cs		\
	Status.cs		\
	StatusFile.cs		\
	WorkingDirectory.cs

Gitty.Lib.CommandLine: $(SOURCES)
	$(CSC) -out:Gitty.Lib.CommandLine.dll -target:library $(SOURCES) -r:../External/Autofac.dll -r:../Gitty.Lib.CSharp/Gitty.Lib.CSharp.dll -r:../Gitty/Gitty.dll

clean: 
	rm -f *dll
