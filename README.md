# KLib

A quick and dirty tool to sort the Library list in NI's Kontakt.
First execute:

    klib export > filename.txt

 Then edit:

    notepad filename.txt

and put the libraries in the desired order.

Finally execute:

    klib import < filename.txt

That's it.
Only supports K5.x. It might work in other versions. But I don't know.