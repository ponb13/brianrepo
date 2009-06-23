(BEGIN)
//set i counter to end of screen memory
@8191
D=A
@i
M=D
//if (M[24576] ==0 then whiten)
@24576
D=M
@WHITEN
D;JEQ

//else (M[24576] !=0) then blacken
@BLACKEN
0;JMP

(WHITEN)
	@i 
	D=M
	@SCREEN
	A=A+D
	M=0
	//decrement i
	@i
	M=M-1
	//check if i == 0 then jmp to begin
	@i
	D=M
	@BEGIN
	D;JEQ
	//else keep whitening
	@WHITEN
	0;JMP
(BLACKEN)
	@i 
	D=M
	@SCREEN
	A=A+D
	M=-1
	//decrement i
	@i
	M=M-1
	//check if i == 0 then jmp to begin
	@i
	D=M
	@BEGIN
	D;JEQ
	//else keep whitening
	@BLACKEN
	0;JMP
	
	
	
	