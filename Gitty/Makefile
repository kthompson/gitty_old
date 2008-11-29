CSC = gmcs

SOURCES = 			\
	Enforce.cs		\
	Git.cs			\
	GitContext.cs		\
	GitModule.cs		\
	IDiffFilesFile.cs	\
	IDiffIndexFile.cs	\
	IFile.cs		\
	IGit.cs			\
	IIndex.cs		\
	IIndexFile.cs		\
	ILsFilesFile.cs		\
	IPath.cs		\
	IRepository.cs		\
	IRepositoryFile.cs	\
	IStatus.cs		\
	IStatusFile.cs		\
	IWorkingDirectory.cs	\
	IWorkingDirectoryFile.cs

Gitty.dll: $(SOURCES)
	$(CSC) -out:Gitty.dll -target:library $(SOURCES) -r:../External/Autofac.dll -r:../Gitty.Lib.CSharp/Gitty.Lib.CSharp.dll

clean: 
	rm -f *dll
