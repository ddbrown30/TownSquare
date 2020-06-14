/*
 * This Modal Dialog box is based heavily on the tutorials of Adam Buckner, located at:
 * 
 * https://unity3d.com/learn/tutorials/modules/intermediate/live-training-archive/modal-window
 * https://unity3d.com/learn/tutorials/modules/intermediate/live-training-archive/modal-window-pt2 and
 * https://unity3d.com/learn/tutorials/modules/intermediate/live-training-archive/modal-window-pt3
 *
 * I took the tutorial example and modified it so that there is one simple function that governs
 * everything.  The syntax is as follows:
 * 
 *     ModalPanel.MessageBox(icon, "Title", "Message", YesFunction, NoFunction, CancelFunction, OkFunction, boolean, type);
 * 
 * The icons need to be defined in the top of your class (as they are in TestModalWindow.cs).  You can have no icons,
 * one icon, or a dozen; it's up to you.  You can reference them directly, as I have done, or any other way that works
 * for you.
 * 
 * Both the title and message are text strings, and can be represented by variables if you so desire.
 *
 * The "YesFunction," "NoFunction," "CancelFunction" and "OkFunction" items can be renamed to whatever you want -
 * just make certain you have the functions in your Class, and that the names make sense to you.  Whatever it
 * is that you want to happen can happen in these functions, or you can pass variables to other functions or
 * other scripts.  Whatever you want to do, it's really easy.
 * 
 * The boolean variable controls the display of the icon.  If true, the icon will display; if false, it will not.
 * 
 * Finally, the type is a string "YesNoCancel" or "YesNo" or "Ok".   These strings control how the Modal Window will
 * pop up and be formatted.  Again, you can add more if you desire; just make sure to create the proper "if" statements
 * under ModalPanel.cs.
 * 
 * The images included with this program are my own creation, and may be used free of fees, licensing, reference, etc., with
 * the exception of the DSoft Logo, which is Copyright (c) 2016 DSoft, Inc. and may not be used without prior written consent.
 *
 * Please note:  You should remove both the TEST Panel and Display Text items for your program.  They are here for testing
 * purposes only.  This also means that you would delete any references to DisplayManager and DisplayManager.DisplayMessage,
 * and you would be able to delete the DisplayManager.cs file.
 * 
 * Finally:  I've gotten tired of finding very little in the Asset Store that was free.  I mean, there have to be a LOT of
 * people out there who, like me, simply can't afford to spend even $3 or $4 to buy a program, especially when it's just a
 * piece of code for a larger program - and without being able to ascertain, for certain, if the piece of code is going to
 * work for you or not.  Therefore, I've made this free.  First of all, it's not originally my code anyway (see above).
 * Second, I want people distributing this, without license, etc., so that we can all "get on with it" for our programming.
 * Of course, Modal Dialog Boxes are something Unity needs to incorporate into its software anyway, but until they do...
 * Well, enjoy.
 * 
 */