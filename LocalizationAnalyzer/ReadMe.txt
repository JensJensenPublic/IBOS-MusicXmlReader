The LocalizationAnalyzer project/assembly can be used in 2 different vays:
1) As an integrated part of the "MusicXmlReader" application-project in the current solution. 
2) As a loosely coupled part of an external project, for instance the "LocalazationAnalyzerCmd" project in the "MusicXmlReaderDeveloperTools" solution.

These 2 senarios are axplained below:


As an integrated part of the "MusicXmlReader" project in the current solution.
The purpose is in this case to emulate the access to the localization resource files are seen from the MusicXmlReader application and thus experienced by the end user.
For this reason everything is deliberately kept unchanged with respect to the normal user situation.
For this reason all localization files are referenced by the "MusicXmlReader"  application and thus loaded from the start of the analyzes.
The LocalyzationAnalyzer is activated from the "MusicXmlReader->Tools->Analyze Localization" menu.
The result is presented as .txt files in the "C:\Users\<User>\AppData\Local\Temp\MusicXmlReader\LocalizationAnalyzer" directory.
DeveloperTool.BrailleMusicDecoder.txt
DeveloperTool.MusicSynthesis.txt
DeveloperTool.MusicXmlReaderHelp.txt
DeveloperTool.MusicXmlReaderModel.txt
DeveloperTool.MusicXmlReaderSettings.txt
DeveloperTool.MusicXmlReaderUI.txt
GlobalLog.txt

The "DeveloperTool" files each contain a matrix of texts. Each row contains a text, each coloumn contains a culture.
The GlobalLog file contains a log generated during the analysis.
 

As a loosely coupled part of an external project

In this case the language resource files are deliberately NOT referenced by the application and must thus be loaded by request by the application.
Thus coml´plicate the code of the owning application, but makes it possible to investigate how the .Net Localization implementation handles loading of resource files,
especially the handling of missing resource files or missing entries in existing resource files!

