using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.DwayneNeed.Win32.Gdi32
{
    public enum COLOR : int
    {
        /// <summary>
        /// Scroll bar gray area.
        /// </summary>
        SCROLLBAR = 0,

        /// <summary>
        /// Desktop.
        /// </summary>
        DESKTOP = 1,

        /// <summary>
        /// Desktop.
        /// </summary>
        BACKGROUND = 1,

        /// <summary>
        /// Active window title bar. 
        /// </summary>
        /// <remarks>
        /// Specifies the left side color in the color gradient of an active
        /// window's title bar if the gradient effect is enabled.
        /// </remarks>
        ACTIVECAPTION = 2,

        /// <summary>
        /// Inactive window caption. 
        /// </summary>
        /// <remarks>
        /// Specifies the left side color in the color gradient of an inactive
        /// window's title bar if the gradient effect is enabled.
        /// </remarks>
        INACTIVECAPTION = 3,

        /// <summary>
        /// Menu background.
        /// </summary>
        MENU = 4,

        /// <summary>
        /// Window background.
        /// </summary>
        WINDOW = 5,

        /// <summary>
        /// Window frame.
        /// </summary>
        WINDOWFRAME = 6,

        /// <summary>
        /// Text in menus.
        /// </summary>
        MENUTEXT = 7,

        /// <summary>
        /// Text in windows.
        /// </summary>
        WINDOWTEXT = 8,

        /// <summary>
        /// Text in caption, size box, and scroll bar arrow box.
        /// </summary>
        CAPTIONTEXT = 9,

        /// <summary>
        /// Active window border.
        /// </summary>
        ACTIVEBORDER = 10,

        /// <summary>
        /// Inactive window border.
        /// </summary>
        INACTIVEBORDER = 11,

        /// <summary>
        /// Background color of multiple document interface (MDI) applications.
        /// </summary>
        APPWORKSPACE = 12,

        /// <summary>
        /// Item(s) selected in a control.
        /// </summary>
        HIGHLIGHT = 13,

        /// <summary>
        /// Text of item(s) selected in a control.
        /// </summary>
        HIGHLIGHTTEXT = 14,

        /// <summary>
        /// Face color for three-dimensional display elements and for dialog
        /// box backgrounds.
        /// </summary>
        BTNFACE = 15,

        /// <summary>
        /// Face color for three-dimensional display elements and for dialog box backgrounds.
        /// </summary>
        /// <remarks>
        /// The actual Win32 name is 3DFACE, but renamed because identifiers
        /// must not start with a digit.
        /// </remarks>
        FACE3D = 15,

        /// <summary>
        /// Shadow color for three-dimensional display elements (for edges
        /// facing away from the light source).
        /// </summary>
        SHADOW3D = 16,

        /// <summary>
        /// Shadow color for three-dimensional display elements (for edges
        /// facing away from the light source).
        /// </summary>
        BTNSHADOW = 16,

        /// <summary>
        /// Grayed (disabled) text. This color is set to 0 if the current
        /// display driver does not support a solid gray color.
        /// </summary>
        GRAYTEXT = 17,

        /// <summary>
        /// Text on push buttons.
        /// </summary>
        BTNTEXT = 18,

        /// <summary>
        /// Color of text in an inactive caption.
        /// </summary>
        INACTIVECAPTIONTEXT = 19,

        /// <summary>
        /// Highlight color for three-dimensional display elements (for edges
        /// facing the light source).
        /// </summary>
        BTNHIGHLIGHT = 20,

        /// <summary>
        /// Highlight color for three-dimensional display elements (for edges
        /// facing the light source).
        /// </summary>
        BTNHILIGHT = 20,

        /// <summary>
        /// Highlight color for three-dimensional display elements (for edges
        /// facing the light source).
        /// </summary>
        HIGHLIGHT3D = 20,
 
        /// <summary>
        /// Highlight color for three-dimensional display elements (for edges facing
        /// the light source).
        /// </summary>
        HILIGHT3D = 20,

        /// <summary>
        /// Dark shadow for three-dimensional display elements.
        /// </summary>
        /// <remarks>
        /// The actual Win32 name is 3DDKSHADOW, but renamed because 
        /// identifiers must not start with a digit.
        /// </remarks>
        DKSHADOW3D = 21,

        /// <summary>
        /// Light color for three-dimensional display elements (for edges
        /// facing the light source).
        /// </summary>
        LIGHT3D = 22,

        /// <summary>
        /// Text color for tooltip controls.
        /// </summary>
        INFOTEXT = 23,

        /// <summary>
        /// Background color for tooltip controls.
        /// </summary>
        INFOBK = 24,

        /// <summary>
        /// Color for a hyperlink or hot-tracked item.
        /// </summary>
        HOTLIGHT = 26,

        /// <summary>
        /// Right side color in the color gradient of an active window's title
        /// bar. ACTIVECAPTION specifies the left side color. Use
        /// SPI.GETGRADIENTCAPTIONS with the SystemParametersInfo function to
        /// determine whether the gradient effect is enabled.
        /// </summary>
        GRADIENTACTIVECAPTION = 27,
        
        /// <summary>
        /// Right side color in the color gradient of an inactive window's
        /// title bar. INACTIVECAPTION specifies the left side color.
        /// </summary>
        GRADIENTINACTIVECAPTION = 28,

        /// <summary>
        /// The color used to highlight menu items when the menu appears as a
        /// flat menu (see SystemParametersInfo). The highlighted menu item is
        /// outlined with HIGHLIGHT.
        /// </summary>
        /// <remarks>
        /// Windows 2000:  This value is not supported.
        /// </remarks>
        MENUHILIGHT = 29, 
 
        /// <summary>
        /// The background color for the menu bar when menus appear as flat
        /// menus (see SystemParametersInfo). However, MENU continues to
        /// specify the background color of the menu popup.
        /// </summary>
        /// <remarks>
        /// Windows 2000:  This value is not supported.
        /// </remarks>
        MENUBAR = 30, 
    }
}
