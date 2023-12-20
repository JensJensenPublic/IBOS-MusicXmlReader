rem This file is run from <SolutionDir>\LibLouisWrapper\bin\Debug
rem Copy the whole LibLouis directory from  ThirdPartyDlls directory (which is controlled by Git) to the Debug Directory.
rem The LibLouisWrapper project expects to find i there.

dir ..\..\..\ThirdPartyDlls\liblouis 

mkdir liblouis

xcopy ..\..\..\ThirdPartyDlls\liblouis liblouis /s/e/v

dir liblouis

pause

exit