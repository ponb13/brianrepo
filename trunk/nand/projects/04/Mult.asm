//check for zeros
////////////////
@R0 //D=R0
D=M

@ZERODETECTED
D;JEQ

@R1 //D=R1
D=M

@ZERODETECTED
D;JEQ
/////////////////


//set sum = 0
@sum
M=0;

(BEGINLOOP)
	//decrement R0
	@R0
	D=M-1
	@R0
	M=D
	
	//add R1 to current sum
	@R1
	D=M
	@sum
	M=M+D
	
	//if R0 = 0 end
	@R0
	D=M
	@ENDLOOP
	D;JEQ
	//else jump to beginloop
	@BEGINLOOP
	0;JMP
(ENDLOOP)


	
	
	





(ZERODETECTED)
	@R2
	M=0
	@END
	0;JMP
(END) 
	@sum
D=M
@R2
M=D
	@END
	0;JMP

