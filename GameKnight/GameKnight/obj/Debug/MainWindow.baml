   M S B A M L   `   `   `  ����E  AGameKnight, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:GameKnight
GameKnight  5   	   O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5   	       GameKnight.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   *localclr-namespace:GameKnight     5   	   X TXceed.Wpf.Toolkit, Version=3.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5   	   0sys&clr-namespace:System;assembly=mscorlib  5   	   -   5          � 
ResizeMode$  CanMinimize=�5	   	    � Title$ 
MainWindow��5   	   $	��450��6   $	��800��6)    � 	Resources 5         tbMargin  discIdsMargin��%T   &	        &	       u�5   
   0,30,0,0q�6/   u�5   
   0,10,0,0q�64   5      .��5      � ���#222222.+�5   
   �� $
��Left=�6   $��293,38,0,121q�60   $	��212��6F   .+�5      �� -   5        $��NewGame_btn��5      ���	#FF858585���	#FFDCD05F$��18(�6i   $
��Bold"�6w   $��30��6�   $��0,0,0,0q�5       � ToolTip$` ZAdd a new game to the google sheet. Default for all users is 0 (i.e. no one owns the game)��5      .��6z   Add New Game6�   �� 5      -   5        $��RemoveGame_btn��5      ���	#FF858585���	#FF0D38BF$��18(�6l   $
��Bold"�6z   $��0,10,0,0q�5      $��30��5      $D >Delete a game from the sheet, all ownership data will be lost!��6!   .��6j   Remove Game6w   �� 5      -   5        $��NewUser_btn��5      ���	#FF858585���	#FFDCD05F$��18(�6i   $
��Bold"�6w   $��30��6�   $R LAdd a new user to the google sheet. This requires a nickname and Discord ID.��5      .��6l   Add New User6z   �� 5      -   5        $��RemoveUser_btn��5      ���	#FF858585���	#FF0D38BF$��18(�6l   $
��Bold"�6z   $��0,10,0,0q�5      $��30��5      $R LDelete a user from the sheet, all ownership data for this user will be lost!��6!   .��6x   Remove User6�   �� 5      -   5        $��CheckOwnership_btn��5      ���	#FF858585���	#FFDCD05F$��18(�6o   $
��Bold"�6}   $��30��6�   $&  Check who owns a specified game.��5      .��6@   Check Game Ownership6V    5       �� 
TargetType �� Property �� Value��%R   '��       7   �� 5       " ��6   .�5!      ��  � ��8   6/   5"      5#      5$      �� 5%   
   $��1��6   .+�5&      �� -   5        $��38��5&   +   $��38��66   $��0q�6B   $G ARefresh - Sync any manual changes you've made to the Google Sheet��5'       �� Left$	 651��6a   
 �� Top$	 358��6s   ��5(      �� 5)      	 �� ImageSource$	 Images/temp_sync.png��6!   
 j� Stretch$
 Uniform=�6D   5+      5,      �� 5-   
   $��1��6   .+�5.      �� -   5        $��45��5.   (   $��42��63   $��0q�6?   $= 7Killswitch - End a poll by killing Game Knight process.��5/      $	 710��6W   $	 356��6i   ��50      �� 51      $	 Images/killbutton.png��6!   $
 Uniform=�6E   53      54      �� 55   
   $��628,0,10,271q�6   .+�56      �� ���	#FFE0E0E0$��14(�6+   $
��Bold"�69   $��29��6K   $��0,0,0,5q�6W   .��6h   Application Options6}   �� 57      -	   5        $��IncludeEveryone_chbx��57      ���#f0ede8$
��Bold"�6H   $^ XCheck this box if you want to ensure everyone owns the games that are put on the ballot.��6Z   $��0,5,0,0q�58       d� 	IsChecked$
 TrueY�6(   .��6   Include Everyone6�   �� 59      -
   5        $��UseBallot_chbx��59      ���#f0ede8$
��Bold"�6B   $��12(�6T   $? 9Check this box if you want more than 1 game to vote from.��6b   $��0,15,0,0q�5:      $
 TrueY�6)   .��6~   
Use Ballot6�   �� 5;      ���	#FFE0E0E0$
��Bold"�6+   .��6=   Total Games on Ballot:6U   �� 5<      -   5         ��Name$ BallotNum_cmbx��5<      $��0,0,0,0q�6/   ���	#FFE2E2E2���	#FFE2E2E2���#333231$
��Bold"�6�   $��12(�6�   $��23��6�   .@�5=      ��  �� 
IsSelected . $��2��61   ���	#FFE2E2E2���	#FFE2E2E2���#333231$
��Bold"�6�   $��12(�6�   �� 5>       .  $��3��62   ���	#FFE2E2E2���	#FFE2E2E2���#333231$
��Bold"�6�   $��12(�6�   �� 5?       .  $��4��62   ���	#FFE2E2E2���	#FFE2E2E2���#333231$
��Bold"�6�   $��12(�6�   �� 5@       .  $��5��62   ���	#FFE2E2E2���	#FFE2E2E2���#333231$
��Bold"�6�   $��12(�6�   �� 5A       .  $��6��62   ���	#FFE2E2E2���	#FFE2E2E2���#333231$
��Bold"�6�   $��12(�6�   5B      5C      �� 5D   
   $��628,139,10,10q�6    5E      ��%R   '��       7  �� 5F      " ��6   .�5G      ��  � ��8   6/   5H      5I      .+�5J      �� ���	#FFE0E0E0$��14(�6+   $
��Bold"�69   $��
0, 0,0,-10q�6K   $��Discord IDs��6_   �� 5K      ���	#FFE0E0E0$
��Bold"�6+   $@ :This is the desired role ID of the Game Night Coordinator.��6=   $��Coordinator Role:��6�   �� 5L      -   5         ��Name$ CoordinatorRole_box��5L      ���#333231���#f0ede8$��23��6]    �� TextWrapping$
 Wrap=�6i   $��Role ID Here��6}   5M      �� 5N      ���	#FFE0E0E0$
��Bold"�6+   $[ UThis is the ID of your Game Knight Bot. Ask your bot manager for the bot's Client ID.��6=   $��Game Knight ID:��5O      �� 5P      -   5        $ GameKnightID_box��5P      ���#333231���#f0ede8$��23��6Z   $
 Wrap=�6f   $��Client ID Here��6z   5Q      �� 5R      ���	#FFE0E0E0$
��Bold"�6+   $< 6This is the channel that Game Knight operates through.��6=   $��Game Knight Channel:��6~   �� 5S      -   5        $ GameKnightChannel_box��5S      ���#333231���#f0ede8$��23��6_   $
 Wrap=�6k   $��Channel ID Here��6   5T      5X      �� 5Y   
   $
��Wrap=�6   ���	#FFE0E0E0$��17(�6?   $
��Bold"�6M   $��10,114,673.667,276q�6_   �� 6|   $�Ignore List��6�   6�   �� 5Z   
   $��1��6   .+�5[      �� -   5        $��IgnoreList_box��5[      ���#333231���#f0ede8$	��116��6V   $ 10��6c   $	 144��6t   $	��159��6�   5\      �� 5]   
   $��1��6   .+�5^      �� -   5        $��20��5^   -   $��20��69   $��0q�6D   $( "Add a new game to the Ignore List.��5_      $	 131��6B   $	 117��6T   ��5`      �� 5a      $	 Images/plus.png��6!   $
 Uniform=�6?   5c      �� 5d      -   5        $��20��5d   0   $��20��6<   $��0q�6G   $) #Remove a game from the ignore list.��5e      $	 151��6G   $	 117��6Y   ��5f      �� 5g      $	 Images/minus.png��6!   $
 Uniform=�6@   5i      5j      �� 5k   
   $��1��6   .+�5l      �� $
��Wrap=�6   ���	#FFE0E0E0$��17(�6C   $
��Bold"�6Q   $ 10��6c   $ 10��6t   �� 6�   $�RSVP Options��6�   6�   �� 5m      -   5        $��DateList_box��5m      ���#333231���#f0ede8$��56��6T   $ 10��6`   $ 38��6q   $	��160��5n      $��Hidden=�6"   5o      �� 5p   
   $��1��6   .+�5q      �� -   5        $��Let's Play!��5q   &   $��37(�6<   $
��Bold"�6J   ���	#FFDCD05F���	#FF858585$��69��5r   ?   $	 280��6K   $	 332��6]   $	��239��6n   5s      �� 5t   
   $
��Wrap=�6   ���	#FFC5BC67$��12(�6?   $��175,375,549,24q�6M   $��Viner Hand ITC)�6e   ��5u      Z� �� 60   $�Version 1.0��64   6J   �� 5v   
   $��1��6   .+�5w      �� -   5        $��20��5w   +   $��20��67   $��0q�6B   $% Add a date to the RSVP options.��5x      $	 131��6?   $ 13��6Q   ��5y      �� 5z      $	 Images/plus.png��6!   $
 Uniform=�6?   5|      �� 5}      -   5        $��20��5}   .   $��20��6:   $��0q�6E   $* $Remove a date from the RSVP options.��5~      $	 151��6H   $ 13��6Z   ��5      �� 5�      $	 Images/minus.png��6!   $
 Uniform=�6@   5�      5�      �� 5�   
   -   5         ��Name$ GK_Standing_img��5�      $
��Left=�6)   $	��150��6D   $	��150��6Q   $��-39,301,0,-30q�6]   $	��Top=�6t   $$��Images/GameKnight_Standing.png��6�    J� RenderTransformOrigin$ 0.5,0.5&�6�   �� 5�   
   -   5        $ GK_Title_img��5�      $
��Left=�6&   $	��194��6A   $	��199��6N   $��26,261,0,-34q�6Z   $	��Top=�6p   $'��!Images/GameKnight_Title_Image.png��6�   $ 0.5,0.5&�6�   � 5�   
   .+�5�      (  "System.Windows.Controls.DatePicker  -   5        $��DateSelector_slct��5�      
 �� Now   SelectedDate# Z 62   $
��Left=�6]   $��
176,37,0,0q�6x   $	��Top=�6�   $	��108��6�   $��0q�6�   $��36��6�   $ 0.307,0.519&�6�   �� 5�      $��1��6   .+�5�      �� -   5        $ DateTime_cmbx��5�      ���	#FFE2E2E2���	#FFE2E2E2 �� MaxDropDownHeight$ 90��5�      ���#333231$
��Bold"�6C   $��11(�6U   $��23��5�   L   $	 177��6X   $ 63��6j   $��52��6z   .@�5�      ��  .  $
��1:00��66   ���	#FFE2E2E2���	#FFE2E2E2���#333231$
��Bold"�6�   $��11(�6�   �� 5�       .  $
��2:00��66   ���	#FFE2E2E2���	#FFE2E2E2���#333231$
��Bold"�6�   $��11(�6�   �� 5�       .  $
��3:00��66   ���	#FFE2E2E2���	#FFE2E2E2���#333231$
��Bold"�6�   $��11(�6�   �� 5�       .  $
��4:00��66   ���	#FFE2E2E2���	#FFE2E2E2���#333231$
��Bold"�6�   $��11(�6�   �� 5�       .  $
��5:00��66   ���	#FFE2E2E2���	#FFE2E2E2���#333231$
��Bold"�6�   $��11(�6�   �� 5�       . $
��6:00��65   ���	#FFE2E2E2���	#FFE2E2E2���#333231$
��Bold"�6�   $��11(�6�   �� 5�       .  $
��7:00��66   ���	#FFE2E2E2���	#FFE2E2E2���#333231$
��Bold"�6�   $��11(�6�   �� 5�       .  $
��8:00��66   ���	#FFE2E2E2���	#FFE2E2E2���#333231$
��Bold"�6�   $��11(�6�   �� 5�       .  $
��9:00��66   ���	#FFE2E2E2���	#FFE2E2E2���#333231$
��Bold"�6�   $��11(�6�   �� 5�       .  $��10:00��66   ���	#FFE2E2E2���	#FFE2E2E2���#333231$
��Bold"�6�   $��11(�6�   �� 5�       .  $��11:00��66   ���	#FFE2E2E2���	#FFE2E2E2���#333231$
��Bold"�6�   $��11(�6�   �� 5�       .  $��12:00��66   ���	#FFE2E2E2���	#FFE2E2E2���#333231$
��Bold"�6�   $��11(�6�   5�      �� 5�      -   5        $ 	AMPM_cmbx��5�      ���	#FFE2E2E2���	#FFE2E2E2���#333231$
��Bold"�6r   $��11(�6�   $��23��5�   L   $	 232��6X   $ 63��6j   $��47��6z   .@�5�      ��  . $��PM��65   ���	#FFE2E2E2���	#FFE2E2E2���#333231$
��Bold"�6�   $��11(�6�   �� 5�       .  $��AM��66   ���	#FFE2E2E2���	#FFE2E2E2���#333231$
��Bold"�6�   $��11(�6�   5�      5�      5�      5�      5�      