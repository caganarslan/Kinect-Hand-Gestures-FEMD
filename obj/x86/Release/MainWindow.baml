   M S B A M L   `   `   `  ����   CoordinateMappingBasics-WPFA    ;Microsoft.Samples.Kinect.CoordinateMappingBasics.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     � Title$  Coordinate Mapping Basics��$	��592��$��1008.256�� � 	Resources    MediumGreyBrush �� Color  KinectPurpleBrush  KinectBlueBrush �� 
TargetType �� Property �� Value  SnapshotButton �� 
TargetType �� Orientation ��Name	 �� TextAlignment
 ��Name �� Triggers P� Property P� Value �� 
TargetName  SquareCheckBox �Name��%�  &	        &	      &	 *     '�� ?     &	 t     7  7  &	 e    7  7  �� $ 	#ff6e6e6e���� $ 	#ff52318f���� $ 	#ff00BCF2���� " ��.���  �@J�SnapsToDevicePixels . �� " ��.���  � �� �� " ��.R�� .+��� $ 
Horizontal=�
������ .+��� $ SnapText��#��z��$
	 Left=�$��Center=�8��  $��15(�� $��9,0,0,0q�.+��� $
 
SnapNormal��$��Images\SnapNormal.png��$
��None=�$��Center=��� $
 	SnapHover��$��Images\SnapHover.png��$
��None=�$��Center=�$y�	Collapsed=� P�  �@J�IsMouseOver . . ��� $ 
SnapNormal�� � y�$ 	Collapsed=��� $ 	SnapHover�� � y�$ Visible=��� 8  $ SnapText�� � ���� " ��.���  � �� �� " ��.R�� .+��� $ 
Horizontal=�
������ .+�� $ SquareCheckBoxChecked��.+��� $
 CheckedNormal��$��Images\CheckedNormal.png��$
��None=�$��Center=��� $
 CheckedHover��$��Images\CheckedHover.png��$
��None=�$��Center=�$y�	Collapsed=�� $ SquareCheckBoxUnchecked��$y�	Collapsed=�.+��� $
 UncheckedNormal��$ ��Images\UncheckedNormal.png��$
��None=�$��Center=��� $
 UncheckedHover��$��Images\UncheckedHover.png��$
��None=�$��Center=�$y�	Collapsed=��� $ SquareCheckBoxText��#��z��$
	 Left=�$��Center=�8��  $��15(�$��9,0,0,0q� P�  �@d�	IsChecked$ falseY�. ��� $ SquareCheckBoxChecked�� � y�$ 	Collapsed=��� $ SquareCheckBoxUnchecked�� � y�$ Visible=�P�  �@J�IsMouseOver . . ��� $ CheckedNormal�� � y�$ 	Collapsed=��� $ UncheckedNormal�� � y�$ 	Collapsed=��� $ CheckedHover�� � y�$ Visible=��� $ UncheckedHover�� � y�$ Visible=��� 8  $ SquareCheckBoxText�� � ��.��� -   $��
layoutGrid��$��	10 0 10 0q� � RowDefinitions �� $
��Auto ��� $��* ��� $
��Auto ��� $
��Auto �.+�]� $��0��$��0 0 0 20q�� $��1��.+��� -   $��Backdrop��$��Images\bgWhite.png���� -   $��MaskedColor��$��10,0,422,156q�� -   $��grid1��$
��Left=�$	��241��$��
595,30,0,0q�$	��Top=�$	��357���� -   $��textBox1��$
��Left=�$��61��$��333,346,0,0q� �� TextWrapping$
 Wrap=�$
��NULL��$	��Top=�$	��119���� -   $��buttonScreenshot��$��2��#��[ $��
Screenshot��$
��Auto��$��Right=�$��Center=�$��
10 10 0 10q��� -   $��checkBoxNearMode��$��2��#��[ $��	Near Mode��$
��Auto��$
��Left=�$��Center=�$��
0 10 10 10q��� -	   $��	statusBar��$��3��$��Stretch=�$��Bottom=�
�������#��[  .@��� $��0 0 0 10q�.���� -
   $��statusBarText��$��-1 0 0 0q�JHPress 'Screenshot' to save a screenshot to your 'My Pictures' directory.