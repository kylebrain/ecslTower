using UnityEngine;

public class ButtonContainer : MonoBehaviour
{
    /// <summary>Type of stack for the buttons</summary>
    public enum StackOrientation { Horizontal, Vertical }

    /// <summary>
    /// The margin around each button
    /// </summary>
    public float buttonMargin = 5f;
    /// <summary>
    /// The orientation type of the container (how will the button be laid out)
    /// </summary>
    public StackOrientation orientation = StackOrientation.Vertical;

    /// <summary>
    /// Array of buttons in the container
    /// </summary>
    public GameButton[] buttons;

    /// <summary>The combined heights of the all the buttons with margins included</summary>
    public float CombinedButtonHeight { get; private set; }
    /// <summary>The combined widths of the all the buttons with margins included</summary>
    public float CombinedButtonWidths { get; private set; }
    /// <summary>The height of the tallest button with margins not included</summary>
    public float TallestButtonHeight { get; private set; }
    /// <summary>The width of the tallest button with margins not included</summary>
    public float WidestButtonWidth { get; private set; }

    /// <summary>
    /// Resize the conatiner, then position the buttons to account for the margins
    /// </summary>
    public void ResizeContainerAndPositionButtons()
    {
        if (buttons == null)
        {
            Debug.Log("Buttons array is null in: " + gameObject.name);
            return;
        }

        if (buttons.Length == 0)
        {
            Debug.Log("There are no buttons in the button conatiner: " + gameObject.name);
        }

        // Run the calculations first
        CalculateCombinedTotalHeights();
        CalculateCombinedTotalWidths();
        CalculateTallestHeight();
        CalculateWidestWidth();

        // Then Resize the container
        ResizeContainer();

        // Then position the buttons
        PositionButtons();
    }

    /// <summary>
    /// Resize the container to be able to fully conatain all of the buttons
    /// </summary>
    private void ResizeContainer()
    {
        float containerHeight = 0;
        float containerWidth = 0;

        // If the orientation is vertical then the width of the conatiner needs to be the width of the
        // widest button plus the margin on the left and the right. The height will be the total combined
        // height (which includes the margins already)
        if (orientation == StackOrientation.Vertical)
        {
            containerHeight = CombinedButtonHeight;
            containerWidth = WidestButtonWidth + (buttonMargin * 2);
        }

        // If the orientation is horizontal then the height of the conatiner needs to be the height of 
        // the tallest button plus the margin on the top and bottom. The width will be the total combined
        // width (which includes the margins already)
        if (orientation == StackOrientation.Horizontal)
        {
            containerHeight = TallestButtonHeight + (buttonMargin * 2);
            containerWidth = CombinedButtonWidths;
        }

        // Set the new size
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(containerWidth, containerHeight);
    }

    /// <summary>
    /// Reposition the buttons to handle the margins 
    /// </summary>
    private void PositionButtons()
    {
        RectTransform conatinerRectTransform = GetComponent<RectTransform>();

        float leftPosition = buttonMargin;
        float topPosition = conatinerRectTransform.rect.height - buttonMargin + conatinerRectTransform.anchoredPosition.y;

        foreach (GameButton button in buttons)
        {
            Debug.Log("Button:" + button.name + " Top:" + topPosition + " Left:" + leftPosition);

            RectTransform rectTransform = button.GetComponent<RectTransform>();

            // Set the anchor and pivot to the bottom left corner
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0, 1);

            // Set the new position
            button.transform.localPosition = new Vector3(leftPosition, topPosition, rectTransform.position.z);

            if (orientation == StackOrientation.Vertical)
            {
                // Update the topPosition for the next button
                topPosition -= rectTransform.rect.height + buttonMargin;
            }

            if (orientation == StackOrientation.Horizontal)
            {
                // Update the leftPosition for the next button
                leftPosition += rectTransform.rect.width + buttonMargin;
            }
        }
    }

    /// <summary>
    /// Calculates the combined total of the heights of the buttons and the margins 
    /// between each, above the first button, and below the last button 
    /// (Sets to 0 if no buttons)
    /// </summary>
    private void CalculateCombinedTotalHeights()
    {
        // Reset combined height to 0
        CombinedButtonHeight = 0f;

        foreach (GameButton button in buttons)
        {
            // Add the heights for the top margin
            CombinedButtonHeight += buttonMargin;

            // Add the height of the game button
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            CombinedButtonHeight += rectTransform.rect.height;
        }

        // Add the margin for the bottom of the last button
        if (buttons.Length > 0)
            CombinedButtonHeight += buttonMargin;
    }

    /// <summary>
    /// Calculates the combined total of the widths of the buttons and the margins 
    /// between each, to the left of the first button, and to the right of the last 
    /// button (Sets to 0 if no buttons)
    /// </summary>
    private void CalculateCombinedTotalWidths()
    {
        // Reset the combined width to 0
        CombinedButtonWidths = 0;

        foreach (GameButton button in buttons)
        {
            // Add the margin for the left side
            CombinedButtonWidths += buttonMargin;
            
            // Add the width of the game button
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            CombinedButtonWidths += rectTransform.rect.width;
        }

        // Add the margin for the right of the last button
        if (buttons.Length > 0)
            CombinedButtonWidths += buttonMargin;
    }

    /// <summary>
    /// Calculates the tallest button's height (Doesn't include any margins)
    /// </summary>
    private void CalculateTallestHeight()
    {
        // Reset to 0
        TallestButtonHeight = 0;

        foreach (GameButton button in buttons)
        {
            // Get the height of the button 
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            float buttonHeight = rectTransform.rect.height;

            // If button height is the tallest so far, set it
            if (buttonHeight > TallestButtonHeight)
                TallestButtonHeight = buttonHeight;
        }
    }

    /// <summary>
    /// Calculates the widest button's width (Doesn't include any margins)
    /// </summary>
    private void CalculateWidestWidth()
    {
        // Reset to 0
        WidestButtonWidth = 0;

        foreach (GameButton button in buttons)
        {
            // Get the width of the button
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            float buttonWidth = rectTransform.rect.width;

            // If button width is the widest so far, set it
            if (buttonWidth > WidestButtonWidth)
                WidestButtonWidth = buttonWidth;
        }
    }
}
