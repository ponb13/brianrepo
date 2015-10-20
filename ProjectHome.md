I've had a some emails regarding this repo, if you are a student I advise ignoring my code altogether and figuring out your own solutions, you'll learn a lot more.

I'd actually prefer to remove this repo if students are using it to cheat - as far as I can tell google do not provide a way for me to do this.

Currently working through this book:

http://www1.idc.ac.il/tecs/

Essentially this book takes the reader through building a computer from scratch, first five chapters deal with creating the hardware (simulated using a Hardware Description Language).

The second half of the book deals with creating an assembler for the platform created in the first half of the book, a virtual machine (stack based), and a compiler that can compile from a high level language down to vm code, which the vm translates into assembly.
The book finishes with the creation of a simple operating system.

All source code is organised by chapter.



I've finished the Assembler (you can just do an svn check out on this url if you are interested):

http://brianrepo.googlecode.com/svn/trunk/nand/projects/06/

And the Virtual Machine (again you can just do an svn check out on this url):

http://brianrepo.googlecode.com/svn/trunk/nand/projects/08/

Currently Working on the tokenizer for the compiler, implemented as a state machine - (work in progress):

http://brianrepo.googlecode.com/svn/trunk/nand/projects/10/
