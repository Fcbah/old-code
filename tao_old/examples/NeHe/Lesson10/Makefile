
DEPTH = ../../..

include $(DEPTH)/config.mk

EXAMPLE = Lesson10
EXAMPLE_CATEGORY = NeHe

SRCS = \
	AssemblyInfo.cs \
	Lesson10.cs \
	$(NULL)

EXAMPLE_DATA = \
	Data/NeHe.Lesson10.Mud.bmp \
	Data/NeHe.Lesson10.World.txt \
	$(NULL)

CSFLAGS += /target:winexe

REFS = \
	Tao.OpenGl.dll \
	Tao.OpenGl.Glu.dll \
	Tao.Platform.Windows.dll \
	System.Windows.Forms.dll \
	System.Drawing.dll \
	$(NULL)

include $(DEPTH)/rules.mk
