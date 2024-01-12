using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;
using static YarnTags;

namespace Yarn
{
    public class TaggedOptionsListView : CustomOptionsListView, IViewable
    {
        public readonly ViewType viewType = ViewType.OptionList;
        private Inclusion viewable = Inclusion.NA;

        public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            if (!ShouldIncludeView(dialogueLine.Metadata, viewType, viewable))
            {
                onDialogueLineFinished();
                return;
            }

            base.RunLine(dialogueLine, onDialogueLineFinished);
        }

        public void SetViewable(Inclusion viewable)
        {
            this.viewable = viewable;
        }

        public ViewType GetViewType()
        {
            return viewType;
        }
    }

    public class CustomOptionsListView : DialogueViewBase
    {
        // UnityEvents
        [Foldout("Events")] public UnityEvent onOptionIsSelected;
        [Foldout("Events")] public UnityEvent onOptionIsHovered;

        [SerializeField] CanvasGroup canvasGroup;

        [SerializeField] CustomOptionView optionViewPrefab;

        [SerializeField] TextMeshProUGUI lastLineText;

        [SerializeField] float fadeTime = 0.1f;

        [SerializeField] bool showUnavailableOptions = false;

        // A cached pool of CustomOptionView objects so that we can reuse them
        List<CustomOptionView> optionViews = new();

        // The method we should call when an option has been selected.
        Action<int> OnOptionSelected;

        // The line we saw most recently.
        LocalizedLine lastSeenLine;

        public void Start()
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        public void Reset()
        {
            canvasGroup = GetComponentInParent<CanvasGroup>();
        }

        public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            // Don't do anything with this line except note it and
            // immediately indicate that we're finished with it. RunOptions
            // will use it to display the text of the previous line.
            lastSeenLine = dialogueLine;
            onDialogueLineFinished();
        }

        public override void RunOptions(DialogueOption[] dialogueOptions, Action<int> onOptionSelected)
        {
            // Hide all existing option views
            foreach (var optionView in optionViews)
            {
                optionView.gameObject.SetActive(false);
            }

            // If we don't already have enough option views, create more
            while (dialogueOptions.Length > optionViews.Count)
            {
                var optionView = CreateNewOptionView();
                optionView.gameObject.SetActive(false);
            }

            // Set up all of the option views
            int optionViewsCreated = 0;

            for (int i = 0; i < dialogueOptions.Length; i++)
            {
                var optionView = optionViews[i];
                var option = dialogueOptions[i];

                if (option.IsAvailable == false && showUnavailableOptions == false)
                {
                    // Don't show this option.
                    continue;
                }

                optionView.gameObject.SetActive(true);

                optionView.Option = option;

                // The first available option is selected by default
                if (optionViewsCreated == 0)
                {
                    optionView.Select();
                }

                optionViewsCreated += 1;
            }

            // Update the last line, if one is configured
            if (lastLineText != null)
            {
                if (lastSeenLine != null)
                {
                    lastLineText.gameObject.SetActive(true);
                    lastLineText.text = lastSeenLine.Text.Text;
                }
                else
                {
                    lastLineText.gameObject.SetActive(false);
                }
            }

            // Note the delegate to call when an option is selected
            OnOptionSelected = onOptionSelected;

            // Fade it all in
            StartCoroutine(Effects.FadeAlpha(canvasGroup, 0, 1, fadeTime));

            /// <summary>
            /// Creates and configures a new <see cref="CustomOptionView"/>, and adds
            /// it to <see cref="optionViews"/>.
            /// </summary>
            CustomOptionView CreateNewOptionView()
            {
                var optionView = Instantiate(optionViewPrefab);
                optionView.transform.SetParent(transform, false);
                optionView.transform.SetAsLastSibling();

                optionView.OnOptionSubmitted = OptionViewWasSelected;
                optionView.OnOptionHovered = OptionViewWasHovered;
                optionViews.Add(optionView);

                return optionView;
            }

            /// <summary>
            /// Called by <see cref="CustomOptionView"/> objects.
            /// </summary>
            void OptionViewWasSelected(DialogueOption option)
            {
                print("Option View Was Selected!");
                onOptionIsSelected.Invoke();

                StartCoroutine(OptionViewWasSelectedInternal(option));

                IEnumerator OptionViewWasSelectedInternal(DialogueOption selectedOption)
                {
                    yield return StartCoroutine(Effects.FadeAlpha(canvasGroup, 1, 0, fadeTime));
                    OnOptionSelected(selectedOption.DialogueOptionID);
                }
            }

            /// <summary>
            /// Called by <see cref="CustomOptionView"/> objects.
            /// </summary>
            void OptionViewWasHovered()
            {
                print("Option View Was Hovered");
                onOptionIsHovered.Invoke();
            }
        }

        /// <inheritdoc />
        /// <remarks>
        /// If options are still shown dismisses them.
        /// </remarks>
        public override void DialogueComplete()
        {
            // do we still have any options being shown?
            if (canvasGroup.alpha > 0)
            {
                StopAllCoroutines();
                lastSeenLine = null;
                OnOptionSelected = null;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;

                StartCoroutine(Effects.FadeAlpha(canvasGroup, canvasGroup.alpha, 0, fadeTime));
            }
        }
    }
}