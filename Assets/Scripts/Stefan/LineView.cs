/*
Yarn Spinner is licensed to you under the terms found in the file LICENSE.md.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;


namespace Dialogue
{

    public class LineAttribute
    {
        public readonly int Position;
        public readonly string Protocol;

        public LineAttribute(int position, string name)
        {
            Position = position;
            Protocol = name;
        }
    }
    [System.Serializable]
    public class DialogueLine
    {
        public string characterName;
        public string text;

        public DialogueLine(string characterName, string text)
        {
            this.characterName = characterName;
            this.text = text;
        }

        public string GetCombinedText()
        {
            
            return characterName + ": " + text ;
        }

        public List<LineAttribute> GetAttributes()
        {
            var results = new List<LineAttribute>();
            var matches = Regex.Matches(text, @"\[(.*?)\]");
            int i = 0;
            foreach (Match match in matches)
            {
                results.Add(new LineAttribute(i++, match.Groups[1].Value));
            }

            return results;
        }
    }

    /// <summary>
    /// A Dialogue View that presents lines of dialogue, using Unity UI
    /// elements.
    /// </summary>
    public class LineView : MonoBehaviour
    {
        /// <summary>
        /// The canvas group that contains the UI elements used by this Line
        /// View.
        /// </summary>
        /// <remarks>
        /// If <see cref="useFadeEffect"/> is true, then the alpha value of this
        /// <see cref="CanvasGroup"/> will be animated during line presentation
        /// and dismissal.
        /// </remarks>
        /// <seealso cref="useFadeEffect"/>
        public CanvasGroup canvasGroup;

        /// <summary>
        /// Controls whether the line view should fade in when lines appear, and
        /// fade out when lines disappear.
        /// </summary>
        /// <remarks><para>If this value is <see langword="true"/>, the <see
        /// cref="canvasGroup"/> object's alpha property will animate from 0 to
        /// 1 over the course of <see cref="fadeInTime"/> seconds when lines
        /// appear, and animate from 1 to zero over the course of <see
        /// cref="fadeOutTime"/> seconds when lines disappear.</para>
        /// <para>If this value is <see langword="false"/>, the <see
        /// cref="canvasGroup"/> object will appear instantaneously.</para>
        /// </remarks>
        /// <seealso cref="canvasGroup"/>
        /// <seealso cref="fadeInTime"/>
        /// <seealso cref="fadeOutTime"/>
        public bool useFadeEffect = true;

        /// <summary>
        /// The time that the fade effect will take to fade lines in.
        /// </summary>
        /// <remarks>This value is only used when <see cref="useFadeEffect"/> is
        /// <see langword="true"/>.</remarks>
        /// <seealso cref="useFadeEffect"/>
        [Min(0)]
        public float fadeInTime = 0.25f;

        /// <summary>
        /// The time that the fade effect will take to fade lines out.
        /// </summary>
        /// <remarks>This value is only used when <see cref="useFadeEffect"/> is
        /// <see langword="true"/>.</remarks>
        /// <seealso cref="useFadeEffect"/>
        [Min(0)]
        public float fadeOutTime = 0.05f;

        /// <summary>
        /// The <see cref="TextMeshProUGUI"/> object that displays the text of
        /// dialogue lines.
        /// </summary>
        public TextMeshProUGUI lineText = null;

        /// <summary>
        /// Controls whether the <see cref="lineText"/> object will show the
        /// character name present in the line or not.
        /// </summary>
        /// <remarks>
        /// <para style="note">This value is only used if <see
        /// cref="characterNameText"/> is <see langword="null"/>.</para>
        /// <para>If this value is <see langword="true"/>, any character names
        /// present in a line will be shown in the <see cref="lineText"/>
        /// object.</para>
        /// <para>If this value is <see langword="false"/>, character names will
        /// not be shown in the <see cref="lineText"/> object.</para>
        /// </remarks>
        [UnityEngine.Serialization.FormerlySerializedAs("showCharacterName")]
        public bool showCharacterNameInLineView = true;

        /// <summary>
        /// The <see cref="TextMeshProUGUI"/> object that displays the character
        /// names found in dialogue lines.
        /// </summary>
        /// <remarks>
        /// If the <see cref="LineView"/> receives a line that does not contain
        /// a character name, this object will be left blank.
        /// </remarks>
        public TextMeshProUGUI characterNameText = null;

        /// <summary>
        /// The gameobject that holds the <see cref="characterNameText"/> textfield.
        /// </summary>
        /// <remarks>
        /// This is needed in situations where the character name is contained within an entirely different game object.
        /// Most of the time this will just be the same gameobject as <see cref="characterNameText"/>.
        /// </remarks>
        public GameObject characterNameContainer = null;

        /// <summary>
        /// Controls whether the text of <see cref="lineText"/> should be
        /// gradually revealed over time.
        /// </summary>
        /// <remarks><para>If this value is <see langword="true"/>, the <see
        /// cref="lineText"/> object's <see
        /// cref="TMP_Text.maxVisibleCharacters"/> property will animate from 0
        /// to the length of the text, at a rate of <see
        /// cref="typewriterEffectSpeed"/> letters per second when the line
        /// appears. <see cref="onCharacterTyped"/> is called for every new
        /// character that is revealed.</para>
        /// <para>If this value is <see langword="false"/>, the <see
        /// cref="lineText"/> will all be revealed at the same time.</para>
        /// <para style="note">If <see cref="useFadeEffect"/> is <see
        /// langword="true"/>, the typewriter effect will run after the fade-in
        /// is complete.</para>
        /// </remarks>
        /// <seealso cref="lineText"/>
        /// <seealso cref="onCharacterTyped"/>
        /// <seealso cref="typewriterEffectSpeed"/>
        public bool useTypewriterEffect = false;

        /// <summary>
        /// A Unity Event that is called each time a character is revealed
        /// during a typewriter effect.
        /// </summary>
        /// <remarks>
        /// This event is only invoked when <see cref="useTypewriterEffect"/> is
        /// <see langword="true"/>.
        /// </remarks>
        /// <seealso cref="useTypewriterEffect"/>
        public UnityEngine.Events.UnityEvent onCharacterTyped;

        /// <summary>
        /// A Unity Event that is called when a pause inside of the typewriter effect occurs.
        /// </summary>
        /// <remarks>
        /// This event is only invoked when <see cref="useTypewriterEffect"/> is <see langword="true"/>.
        /// </remarks>
        /// <seealso cref="useTypewriterEffect"/>
        public UnityEngine.Events.UnityEvent onPauseStarted;
        /// <summary>
        /// A Unity Event that is called when a pause inside of the typewriter effect finishes and the typewriter has started once again.
        /// </summary>
        /// <remarks>
        /// This event is only invoked when <see cref="useTypewriterEffect"/> is <see langword="true"/>.
        /// </remarks>
        /// <seealso cref="useTypewriterEffect"/>
        public UnityEngine.Events.UnityEvent onPauseEnded;

        /// <summary>
        /// The number of characters per second that should appear during a
        /// typewriter effect.
        /// </summary>
        /// <seealso cref="useTypewriterEffect"/>
        [Min(0)]
        public float typewriterEffectSpeed = 0f;

        /// <summary>
        /// The game object that represents an on-screen button that the user
        /// can click to continue to the next piece of dialogue.
        /// </summary>
        /// <remarks>
        /// <para>This game object will be made inactive when a line begins
        /// appearing, and active when the line has finished appearing.</para>
        /// <para>
        /// This field will generally refer to an object that has a <see
        /// cref="Button"/> component on it that, when clicked, calls <see
        /// cref="OnContinueClicked"/>. However, if your game requires specific
        /// UI needs, you can provide any object you need.</para>
        /// </remarks>
        /// <seealso cref="autoAdvance"/>
        public GameObject continueButton = null;

        /// <summary>
        /// The amount of time to wait after any line
        /// </summary>

        [Min(0)]
        public float holdTime = 1f;

        /// <summary>
        /// Controls whether this Line View will wait for user input before
        /// indicating that it has finished presenting a line.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If this value is true, the Line View will not report that it has
        /// finished presenting its lines. Instead, it will wait until the <see
        /// cref="UserRequestedViewAdvancement"/> method is called.
        /// </para>
        /// <para style="note"><para>The <see cref="DialogueRunner"/> will not
        /// proceed to the next piece of content (e.g. the next line, or the
        /// next options) until all Dialogue Views have reported that they have
        /// finished presenting their lines. If a <see cref="LineView"/> doesn't
        /// report that it's finished until it receives input, the <see
        /// cref="DialogueRunner"/> will end up pausing.</para>
        /// <para>
        /// This is useful for games in which you want the player to be able to
        /// read lines of dialogue at their own pace, and give them control over
        /// when to advance to the next line.</para></para>
        /// </remarks>
        public bool autoAdvance = false;

        /// <summary>
        /// The current <see cref="LocalizedLine"/> that this line view is
        /// displaying.
        /// </summary>
        DialogueLine currentLine = null;

        /// <summary>
        /// A stop token that is used to interrupt the current animation.
        /// </summary>
        readonly Effects.CoroutineInterruptToken currentStopToken = new();

        private void Awake()
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
        }

        private void Reset()
        {
            canvasGroup = GetComponentInParent<CanvasGroup>();
        }

        public void DismissLine()
        {
            DismissLine(null);
        }
        public void DismissLine(Action onDismissalComplete)
        {
            currentLine = null;

            StartCoroutine(DismissLineInternal(onDismissalComplete));
        }

        private IEnumerator DismissLineInternal(Action onDismissalComplete)
        {
            // disabling interaction temporarily while dismissing the line
            // we don't want people to interrupt a dismissal
            var interactable = canvasGroup.interactable;
            canvasGroup.interactable = false;

            // If we're using a fade effect, run it, and wait for it to finish.
            if (useFadeEffect)
            {
                yield return StartCoroutine(Effects.FadeAlpha(canvasGroup, 1, 0, fadeOutTime, currentStopToken));
                currentStopToken.Complete();
            }

            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            // turning interaction back on, if it needs it
            canvasGroup.interactable = interactable;

            onDismissalComplete?.Invoke();
        }

        public void InterruptLine(DialogueLine dialogueLine, Action onInterruptLineFinished)
        {
            currentLine = dialogueLine;

            // Cancel all coroutines that we're currently running. This will
            // stop the RunLineInternal coroutine, if it's running.
            StopAllCoroutines();

            // for now we are going to just immediately show everything
            // later we will make it fade in
            lineText.gameObject.SetActive(true);
            canvasGroup.gameObject.SetActive(true);

            int length;

            if (characterNameText == null)
            {
                if (showCharacterNameInLineView)
                {
                    lineText.text = dialogueLine.GetCombinedText();
                    length = lineText.text.Length;
                }
                else
                {
                    lineText.text = dialogueLine.text;
                    length = dialogueLine.text.Length;
                }
            }
            else
            {
                characterNameText.text = dialogueLine.characterName;
                lineText.text = dialogueLine.text;
                length = dialogueLine.text.Length;
            }

            // Show the entire line's text immediately.
            lineText.maxVisibleCharacters = length;

            // Make the canvas group fully visible immediately, too.
            canvasGroup.alpha = 1;

            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            onInterruptLineFinished?.Invoke();
        }

        public void RunLine(DialogueLine dialogueLine, Action onDialogueLineFinished = null)
        {
            // Stop any coroutines currently running on this line view (for
            // example, any other RunLine that might be running)
            StopAllCoroutines();

            // Begin running the line as a coroutine.
            StartCoroutine(RunLineInternal(dialogueLine, onDialogueLineFinished));
        }

        IEnumerator PresentLine(DialogueLine dialogueLine)
        {
            lineText.gameObject.SetActive(true);
            canvasGroup.gameObject.SetActive(true);

            // Hide the continue button until presentation is complete (if
            // we have one).
            if (continueButton != null)
            {
                continueButton.SetActive(false);
            }

            var text = dialogueLine.text;
            if (characterNameContainer != null && characterNameText != null)
            {
                // we are set up to show a character name, but there isn't one
                // so just hide the container
                if (string.IsNullOrWhiteSpace(dialogueLine.characterName))
                {
                    characterNameContainer.SetActive(false);
                }
                else
                {
                    // we have a character name text view, show the character name
                    characterNameText.text = dialogueLine.characterName;
                    characterNameContainer.SetActive(true);
                }
                lineText.text = text;
            }
            else
            {
                // We don't have a character name text view. Should we show
                // the character name in the main text view?
                if (showCharacterNameInLineView)
                {
                    // Yep! Show the entire text.
                    lineText.text = dialogueLine.GetCombinedText();
                }
                else
                    lineText.text = text;
            }


            if (useTypewriterEffect)
            {
                // If we're using the typewriter effect, hide all of the
                // text before we begin any possible fade (so we don't fade
                // in on visible text).
                lineText.maxVisibleCharacters = 0;
            }
            else
            {
                // Ensure that the max visible characters is effectively
                // unlimited.
                lineText.maxVisibleCharacters = int.MaxValue;
            }

            // If we're using the fade effect, start it, and wait for it to
            // finish.
            if (useFadeEffect)
            {
                //yield return StartCoroutine(Effects.FadeAlpha(canvasGroup, 0, 1, fadeInTime, currentStopToken));
                yield return null;
                if (currentStopToken.WasInterrupted)
                {
                    // The fade effect was interrupted. Stop this entire
                    // coroutine.
                    yield break;
                }
            }

            // If we're using the typewriter effect, start it, and wait for
            // it to finish.
            if (useTypewriterEffect)
            {
                var pauses = LineView.GetPauseDurationsInsideLine(dialogueLine);

                // setting the canvas all back to its defaults because if we didn't also fade we don't have anything visible
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;

                yield return StartCoroutine(Effects.PausableTypewriter(
                    lineText,
                    typewriterEffectSpeed,
                    () => onCharacterTyped.Invoke(),
                    () => onPauseStarted.Invoke(),
                    () => onPauseEnded.Invoke(),
                    pauses,
                    currentStopToken
                ));

                if (currentStopToken.WasInterrupted)
                {
                    // The typewriter effect was interrupted. Stop this
                    // entire coroutine.
                    yield break;
                }
            }
        }

        private IEnumerator RunLineInternal(DialogueLine dialogueLine, Action onDialogueLineFinished)
        {
            
            currentLine = dialogueLine;

            // Run any presentations as a single coroutine. If this is stopped,
            // which UserRequestedViewAdvancement can do, then we will stop all
            // of the animations at once.
            yield return StartCoroutine(PresentLine(dialogueLine));

            currentStopToken.Complete();

            // All of our text should now be visible.
            lineText.maxVisibleCharacters = int.MaxValue;

            // Our view should at be at full opacity.
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;

            // Show the continue button, if we have one.
            if (continueButton != null)
            {
                continueButton.SetActive(true);
            }

            // If we have a hold time, wait that amount of time, and then
            // continue.
            if (holdTime > 0)
            {
                yield return new WaitForSeconds(holdTime);
            }

            if (!autoAdvance)
            {
                // The line is now fully visible, and we've been asked to not
                // auto-advance to the next line. Stop here, and don't call the
                // completion handler - we'll wait for a call to
                // UserRequestedViewAdvancement, which will interrupt this
                // coroutine.
                yield break;
            }

            // Our presentation is complete; call the completion handler.
            onDialogueLineFinished?.Invoke();
        }

        public void UserRequestedViewAdvancement()
        {
            // We received a request to advance the view. If we're in the middle of
            // an animation, skip to the end of it. If we're not current in an
            // animation, interrupt the line so we can skip to the next one.

            // we have no line, so the user just mashed randomly
            if (currentLine == null) return;

            // we may want to change this later so the interrupted
            // animation coroutine is what actually interrupts
            // for now this is fine.
            // Is an animation running that we can stop?
            if (currentStopToken.CanInterrupt)
            {
                // Stop the current animation, and skip to the end of whatever
                // started it.
                currentStopToken.Interrupt();
            }
            else
            {
                // No animation is now running. Signal that we want to
                // interrupt the line instead.
                //requestInterrupt?.Invoke();
            }
        }

        /// <summary>
        /// Called when the <see cref="continueButton"/> is clicked.
        /// </summary>
        public void OnContinueClicked()
        {
            // When the Continue button is clicked, we'll do the same thing as
            // if we'd received a signal from any other part of the game (for
            // example, if a DialogueAdvanceInput had signalled us.)

            UserRequestedViewAdvancement();
        }

        /// <remarks>
        /// If a line is still being shown dismisses it.
        /// </remarks>
        public void DialogueComplete()
        {
            // do we still have a line lying around?
            if (currentLine == null) return;
            currentLine = null;
            StopAllCoroutines();
            StartCoroutine(DismissLineInternal(null));
        }

        /// <summary>
        /// Creates a stack of typewriter pauses to use to temporarily halt the typewriter effect.
        /// </summary>
        /// <remarks>
        /// This is intended to be used in conjunction with the <see cref="Effects.PausableTypewriter"/> effect.
        /// The stack of tuples created are how the typewriter effect knows when, and for how long, to halt the effect.
        /// <para>
        /// The pause duration property is in milliseconds but all the effects code assumes seconds
        /// So here we will be dividing it by 1000 to make sure they interconnect correctly.
        /// </para>
        /// </remarks>
        /// <param name="line">The line from which we covet the pauses</param>
        /// <returns>A stack of positions and duration pause tuples from within the line</returns>
        public static Stack<(int position, float duration)> GetPauseDurationsInsideLine(DialogueLine line)
        {
            var pausePositions = new Stack<(int, float)>();
            var label = "pause";

            // sorting all the attributes in reverse positional order
            // this is so we can build the stack up in the right positioning
            var attributes = line.GetAttributes();
            attributes.Sort((a, b) => (b.Position.CompareTo(a.Position)));
            foreach (var attribute in attributes)
            {
                string[] parameters = attribute.Protocol.Split(' ');
                // if we aren't a pause skip it
                if (parameters[0] != label) continue;

                // did they set a custom duration or not, as in did they do this:
                //     Alice: this is my line with a [pause = 1000 /]pause in the middle
                // or did they go:
                //     Alice: this is my line with a [pause /]pause in the middle
                if (float.TryParse(parameters[1], out float value))
                {
                    pausePositions.Push((attribute.Position, value / 1000));
                }
                else
                {
                    // they haven't set a duration, so we will instead use the default of one second
                    pausePositions.Push((attribute.Position, 1));
                }
            }
            return pausePositions;
        }
    }
}
