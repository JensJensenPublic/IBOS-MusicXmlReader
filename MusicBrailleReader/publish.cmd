rem exit

rem This file seems to be run from C:\Users\Jens\Dropbox\Root\Visual Studio 2015\Projects\MusicXmlReaderUI\MusicBrailleReader\bin\Debug

dir "..\..\..\Documentation\Test descriptions\Version 4.1"


rem da-DK----------------------------------------------------------------------------------------------------------------------------------------------
rem dir "Documentation\da-DK"
rem NOTE: The command interpreter running this file can not handle scandinavian letters such as "æ" We fix this problem by using a wildcard instead !
rem NOTE: In order to avoid having old versions of "Brugervejledning" we first clean up 

erase "Documentation\Brugervejledning*.docx"
copy "..\..\..\Documentation\Test descriptions\Version 4.1\Brugervejledning for IBOS Punktnodel*ser.v4.1.docx" "Documentation"

