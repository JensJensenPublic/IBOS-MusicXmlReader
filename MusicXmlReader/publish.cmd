exit

rem This file seems to be run from C:\Users\Jens\Dropbox\Root\Visual Studio 2015\Projects\MusicXmlReaderUI\MusicXmlReader\bin\Debug

rem dir "..\..\..\Documentation\Official Documentation"


rem da-DK----------------------------------------------------------------------------------------------------------------------------------------------
rem dir "Documentation\da-DK"
rem NOTE: The command interpreter running this file can not handle scandinavian letters such as "æ" We fix this problem by using a wildcard instead !
rem NOTE: In order to avoid having old versions of "Brugervejledning" we first clean up 

erase "Documentation\da-DK\Brugervejledning*.doc"
copy "..\..\..\Documentation\Official Documentation\Brugervejledning for IBOS Nodel*ser version 2.0.0.0.doc" "Documentation\da-DK"

rem en-US----------------------------------------------------------------------------------------------------------------------------------------------
rem NOTE: In order to avoid having old versions of "User's manual" we first clean up 
erase "Documentation\en-US\User's manual*.doc"
copy "..\..\..\Documentation\Official Documentation\User's manual for IBOS MusicXmlReader version 2.0.0.0.doc" "Documentation\en-US"
