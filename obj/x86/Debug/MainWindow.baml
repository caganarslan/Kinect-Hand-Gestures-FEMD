   M S B A M L   `   `   `  ����  CoordinateMappingBasics-WPFA    ;Microsoft.Samples.Kinect.CoordinateMappingBasics.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  Coordinate Mapping Basics��5   	   $	��592��6+   $��869.45��68    � 	Resources 5         MediumGreyBrush �� Color  KinectPurpleBrush  KinectBlueBrush �� 
TargetType �� Property �� Value  SnapshotButton �� 
TargetType �� Orientation ��Name	 �� TextAlignment
 ��Name �� Triggers P� Property P� Value �� 
TargetName  SquareCheckBox �Name��%w
  &	        &	 #     &	 F     '�� i     &	 �     7  7  &	 �    7  7  �� 5   
   $ 	#ff6e6e6e��62   �� 5   
   $ 	#ff52318f��64   �� 5	   
   $ 	#ff00BCF2��62   �� 5
   
   " ��6   .�5      ��  �@J�SnapsToDevicePixels . 64   5      �� 5   
   " ��6   .�5      ��  � �� 5      �� 5      " ��6&   .R�5      � .+�5      �� $ 
Horizontal=�6)   
������ .+�5   "   �� $ SnapText��6,   #��z��6>   $
	 Left=�6_   $��Center=�6u   8��  $��15(�6�   � 5   "   $��9,0,0,0q�6'   .+�5   &   �� $
 
SnapNormal��6,   $��Images\SnapNormal.png��6@   $
��None=�6_   $��Center=�6n   �� 5   &   $
 	SnapHover��6,   $��Images\SnapHover.png��6?   $
��None=�6]   $��Center=�6l   $y�	Collapsed=�6�   5   #   5      5       5      P� 5       �@J�IsMouseOver . . �5   "   �� $ 
SnapNormal��6Q    � y�$ 	Collapsed=�6?   6Q   �� 5   "   $ 	SnapHover��6O    � y�$ Visible=�6?   6O   �� 5   "   8  $ SnapText��6h    � ��5      5!      5#      5$      �� 5&   
   " ��6   .�5'      ��  � �� 5(      �� 5)      " ��6&   .R�5*      � .+�5+      �� $ 
Horizontal=�6)   
������ .+�5,   "   � $ SquareCheckBoxChecked��6'   .+�5-   &   �� $
 CheckedNormal��6,   $��Images\CheckedNormal.png��6C   $
��None=�6e   $��Center=�6t   �� 5.   &   $
 CheckedHover��6,   $��Images\CheckedHover.png��6B   $
��None=�6c   $��Center=�6r   $y�	Collapsed=�6�   5/   #   � 50   "   $ SquareCheckBoxUnchecked��6'   $y�	Collapsed=�6H   .+�51   &   �� $
 UncheckedNormal��6,   $ ��Images\UncheckedNormal.png��6E   $
��None=�6i   $��Center=�6x   �� 52   &   $
 UncheckedHover��6,   $��Images\UncheckedHover.png��6D   $
��None=�6g   $��Center=�6v   $y�	Collapsed=�6�   53   #   �� 54   "   $ SquareCheckBoxText��6,   #��z��6H   $
	 Left=�6i   $��Center=�6   8��  $��15(�6�   $��9,0,0,0q�6�   55      56       57      P� 58       �@d�	IsChecked$ falseY�6;   . �59   "   �� $ SquareCheckBoxChecked��6Q    � y�$ 	Collapsed=�6?   6Q   �� 5:   "   $ SquareCheckBoxUnchecked��6O    � y�$ Visible=�6?   6O   5;      P� 5<       �@J�IsMouseOver . . �5=   "   �� $ CheckedNormal��6Q    � y�$ 	Collapsed=�6?   6Q   �� 5>   "   $ UncheckedNormal��6Q    � y�$ 	Collapsed=�6?   6Q   �� 5?   "   $ CheckedHover��6O    � y�$ Visible=�6?   6O   �� 5@   "   $ UncheckedHover��6O    � y�$ Visible=�6?   6O   �� 5A   "   8  $ SquareCheckBoxText��6h    � ��5B      5D      5F      5G      5H      .��5J      � -   5        $��
layoutGrid��5J      $��	10 0 10 0q�6    � RowDefinitions 5K   
   �� 5L      $
��Auto �6   �� 5M      $��* �6   �� 5N      $
��Auto �6   �� 5O      $
��Auto �6   .+�5Q   
   ]� $��0��6   $��0 0 0 20q�6!   � 5R   
   $��1��6   .+�5S      �� -   5        $��Backdrop��5S      $��Images\bgWhite.png��6%   �� 5T      -   5        $��MaskedColor��5T      $��158,0,168,156q�6(   � 5U      -   5        $��grid1��5U      $
��Left=�6    $��84��6;   $��120,337,0,0q�6G   $	��Top=�6\   $	��151��6t   �� 5V      -   5        $��textBox1��5V      $
��Left=�6&   $��61��6A   $��333,346,0,0q�6M    �� TextWrapping$
 Wrap=�6b   $
��NULL��6v   $	��Top=�6�   $	��119��6�   6�   5W      �� 5X   
   -   5        $��buttonScreenshot��5X   �   $��2��6   #��[ 6   $��
Screenshot��6F   $
��Auto��6[   $��Right=�6i   $��Center=�6�   $��
10 10 0 10q�6�   6�   �� 5Y   
   -   5        $��checkBoxNearMode��5Y   �   $��2��6   #��[ 6    $��	Near Mode��6H   $
��Auto��6\   $
��Left=�6j   $��Center=�6�   $��
0 10 10 10q�6�   6�   �� 5Z   
   -	   5        $��	statusBar��5Z   ?   $��3��6   $��Stretch=�6!   $��Bottom=�6P   
�������#��[  6~   .@�5[      �� $��0 0 0 10q�6   .��5\      �� -
   5        $��statusBarText��5\      $��-1 0 0 0q�61   JHPress 'Screenshot' to save a screenshot to your 'My Pictures' directory.6�   5]      5^      5_      5`      