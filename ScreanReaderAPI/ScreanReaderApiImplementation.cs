// JSJ: Code dtolen from http://forum.audiogames.net/viewtopic.php?id=12026

using System;
using System.Speech.Synthesis;
using System.Runtime.InteropServices;
using System.ComponentModel;
    /// <summary>
    /// For this class to work as expected, a 32-bit application should define a conditional variable named x86.
    /// In addition, 32-bit applications should reference JFWAPICTRLLib, and 64-bit applications should reference FSAPILib.
    /// This can be done by adding the COM references under the "References" node in the project solution.
    /// Also, the NVDA API should exist in the same directory as the executable. 32-bit applications should use nvdaControllerClient32.dll and 64-bit applications should use nvdaControllerClient64.dll.
    /// </summary>
    public class ScreenReader
{
#if x86
     [DllImport("nvdaControllerClient32.dll", CharSet = CharSet.Unicode)]
#else
    [DllImport("nvdaControllerClient64.dll", CharSet = CharSet.Unicode)]
#endif
    static extern long nvdaController_speakText(String text);
#if x86
     [DllImport("nvdaControllerClient32.dll", CharSet = CharSet.Unicode)]
#else
    [DllImport("nvdaControllerClient64.dll", CharSet = CharSet.Unicode)]
#endif
    static extern long nvdaController_cancelSpeech();

    /// <summary>
    /// Describes speaking methods. The ones that hault the program are only applicable to SAPI.
    /// </summary>
    public enum SpeakFlag
    {
        noInterrupt,
        noInterruptButStop,
        interruptable,
        interruptableButStop
    }

    /// <summary>
    /// Defines what screen reader to use.
    /// </summary>
    public enum SpeechSource
    {
        SAPI,
        JAWS,
        WE,
        SA,
        NVDA
    }
    private static SpeechSynthesizer voice;
    private static string lastSpokenString = "";
    //For JAWS, 32-bit and 64-bit use different objects.
#if x86
  private static JFWAPICTRLLib.JFWApi JAWS;
#else
    private static FSAPILib.JawsApi JAWS;
#endif
    private static SpeechSource source;
    /// <summary>
    /// Initializes SAPI. This step is optional, but it's recommended to always have
    /// SAPI initialized at least as a fallback.
    /// The method also removes the JAWS keyboard hook.
    /// </summary>
    public static void initialize()
    {
        voice = new SpeechSynthesizer();
        disableJAWSHook();
    }

    /// <summary>
    /// Sets the screen reader to use.
    /// </summary>
    /// <param name="source">The speaking source, such as SpeechSource.JAWS</param>
    public static void setSource(SpeechSource source)
    {
        ScreenReader.source = source;
        if (source == SpeechSource.JAWS)
        {
            if (JAWS == null)
            {
#if x86
          JAWS = new JFWAPICTRLLib.JFWApi();
#else
                JAWS = new FSAPILib.JawsApi();
#endif
            }
            JAWS.Disable();
        }
    }

    /// <summary>
    /// Says something through the speech source.
    /// </summary>
    /// <param name="sayString">The string to speak</param>
    /// <param name="flag">The way the string should be spoken. For example, if noInterrupt was passed, then SAPI would hault the executing thread until it was done speaking. Typically, the flag
    /// only affects SpeechSource.SAPI.</param>
    public static void speak(string sayString, SpeakFlag flag)
    {
        if (flag == SpeakFlag.interruptableButStop || flag == SpeakFlag.noInterruptButStop)
        {
            if (source == SpeechSource.SAPI)
                voice.SpeakAsyncCancelAll();
            else if (source == SpeechSource.JAWS)
                JAWS.StopSpeech();
            else if (source == SpeechSource.NVDA)
                nvdaController_cancelSpeech();
        }

        if (flag == SpeakFlag.noInterrupt || flag == SpeakFlag.noInterruptButStop)
        {
            if (source == SpeechSource.SAPI)
                voice.Speak(sayString);
            else if (source == SpeechSource.JAWS)
            {
#if x86
           JAWS.SayString(sayString, 0);
#else
                JAWS.SayString(sayString, false);
#endif
            }
            else if (source == SpeechSource.NVDA)
                nvdaController_speakText(sayString);
        }
        if (flag == SpeakFlag.interruptable || flag == SpeakFlag.interruptableButStop)
        {
            if (source == SpeechSource.SAPI)
                voice.SpeakAsync(sayString);
            else if (source == SpeechSource.JAWS)
            {
#if x86
           JAWS.SayString(sayString, 0);
#else
                JAWS.SayString(sayString, false);
#endif
            }
            else if (source == SpeechSource.NVDA)
                nvdaController_speakText(sayString);
        }
        lastSpokenString = sayString;
    }

    /// <summary>
    /// Gets the last spoken string.
    /// </summary>
    /// <returns>The last string to be spoken by the speech source</returns>
    public static string getRepeat()
    {
        return lastSpokenString;
    }

    /// <summary>
    /// Applicable to SpeechSource.SAPI only.
    /// </summary>
    /// <returns>True if the TTS is speaking, false otherwise</returns>
    public static bool isSpeaking()
    {
        return voice.State == SynthesizerState.Speaking;
    }

    /// <summary>
    /// Stops speech.
    /// </summary>
    public static void purge()
    {
        try
        {
            if (source == SpeechSource.SAPI)
                voice.SpeakAsyncCancelAll();
            else if (source == SpeechSource.JAWS)
                JAWS.StopSpeech();
            else if (source == SpeechSource.NVDA)
                nvdaController_cancelSpeech();
        }
        catch (System.OperationCanceledException e)
        {
        }
    }


    /// <summary>
    /// This method should be run just before the program shuts down. It will reinstate the JAWS keyboard hook.
    /// </summary>
    public static void cleanUp()
    {
        enableJAWSHook();
    }

    /// <summary>
    /// Removes the JAWS keyboard hook.
    /// </summary>
    private static void disableJAWSHook()
    {
        if (JAWS == null)
        {
#if x86
                JAWS = new JFWAPICTRLLib.JFWApi();
#else
            JAWS = new FSAPILib.JawsApi();
#endif
        }
        JAWS.Disable();
    }

    /// <summary>
    /// Reinstates the JAWS keyboard hook.
    /// </summary>
    private static void enableJAWSHook()
    {
        if (JAWS == null)
            return;
#if x86
                JAWS.Enable(1);
#else
        JAWS.Enable(true);
#endif
    }

}