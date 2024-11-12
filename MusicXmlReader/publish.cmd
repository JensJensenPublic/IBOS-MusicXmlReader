

rem This file is run from <SolutionDir>\MusicXmlReader\bin\Debug

echo --------------------------------------------------------------------------------------------------------------
echo MusicXmlReader.Publish.bat is started
echo Remove the following line, containing the "exit" command in order to update all resources used by the program.
echo --------------------------------------------------------------------------------------------------------------
exit

rem Copy all needed native dlls from the  ThirdPartyDlls directory (which is controlled by Git) to the Debug Directory.
rem The managed dlls need no copying, because they are all explicitly referenced by the source code and thus copied by VS
rem dir ..\..\..\ThirdPartyDlls
copy ..\..\..\ThirdPartyDlls\i386\NAudio.dll
copy ..\..\..\ThirdPartyDlls\i386\fsbrldspapi.dll
copy ..\..\..\ThirdPartyDlls\i386\FSapi.dll
copy ..\..\..\ThirdPartyDlls\i386\jfwapi.dll
copy ..\..\..\ThirdPartyDlls\i386\nvdaControllerClient32.dll
copy ..\..\..\ThirdPartyDlls\7Zip\7z.dll
copy ..\..\..\ThirdPartyDlls\7Zip\7z.exe

rem Copy the icon-file, containing the small icon shown in the upper left corner of the application
copy ..\..\..\Documentation\icon.ico

rem Create an empty directory "\MusicXmlReader\bin\Debug\MusicXml samples" and xcopy all file from ..<Solution>\Tactile MusicXmlReader\MusicXmlSamples there:

rmdir "MusicXml samples" /S/Q

mkdir "MusicXml samples"

xcopy   ..\..\..\MusicXmlSamples "MusicXml samples" /s/e/v

rem Create an empty directory "\MusicXmlReader\bin\Debug\JAWS" and xcopy all file from ..<Solution>\Tactile MusicXmlReader\JAWS there:
rmdir "JAWS" /S/Q
mkdir "JAWS"
xcopy   ..\..\..\JAWS "JAWS" /s/e/v

dir "..\..\..\Documentation\Official Documentation"

rem First create an empty directory "\MusicXmlReader\bin\Debug\Documentation", common for all localization-languages (da-DK, en-US etc)
rmdir "Documentation" /S/Q
mkdir "Documentation"

rem For each language copy the user's' manual

rem da-DK----------------------------------------------------------------------------------------------------------------------------------------------
rmdir "Documentation\da-DK" /S/Q
mkdir "Documentation\da-DK"
dir "Documentation\da-DK"
rem NOTE: The command interpreter running this file can not handle scandinavian letters such as "æ" We fix this problem by using a wildcard instead !
rem NOTE: In order to avoid having old versions of "Brugervejledning" we first clean up 
rem erase "Documentation\da-DK\Brugervejledning*.doc*" /S/Q
copy "..\..\..\Documentation\Official Documentation\Brugervejledning for IBOS Nodel*ser version 5.0.0.0.docx" "Documentation\da-DK"
dir "Documentation\da-DK"

rem en-US----------------------------------------------------------------------------------------------------------------------------------------------
rem NOTE: In order to avoid having old versions of "User's manual" we first clean up 
rmdir "Documentation\en-US" /S/Q
mkdir "Documentation\en-US"
dir "Documentation\en-US"
rem erase "Documentation\en-US\User's manual*.doc*" /S/Q
copy "..\..\..\Documentation\Official Documentation\User's manual for IBOS MusicXmlReader version 5.0.0.0.docx" "Documentation\en-US"
dir "Documentation\en-US"

echo ------------------------------------------------------------
echo The full version of MusicXmlReader.Publish.bat was completed
echo ------------------------------------------------------------

