@echo on
c:
cd \Solutions\ohi\icebg
for /d /r . %%d in (bin,obj,node_modules,.vs) do @if exist "%%d" rd /s /q "%%d"
pause

