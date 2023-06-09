Alias ("Types.Cht", Choice [Bool; Int32])
Alias
  ("Types.Sex",
   Union [UnionMember ("Male", [String]); UnionMember ("Female", [])])
Alias ("Types.Pet", Union [UnionMember ("Cat", []); UnionMember ("Dog", [])])
Alias ("Types.Age", List (AnonRecord [RecordMember ("name", String)]))
Alias
  ("Types.Address",
   Record [RecordMember ("Street", String); RecordMember ("State'", String)])
Alias
  ("Model.Employee",
   Record
     [RecordMember ("Name", String); RecordMember ("Age", Userdef "Types.Age");
      RecordMember ("Address", Option (Userdef "Types.Address"))])
Alias
  ("Model.Person",
   Record
     [RecordMember ("Name", String); RecordMember ("Age", Userdef "Types.Age");
      RecordMember ("Address", Option (Userdef "Types.Address"));
      RecordMember ("Sex", Userdef "Types.Sex");
      RecordMember ("Pet", Option (Userdef "Types.Pet"));
      RecordMember ("Cht", Userdef "Types.Cht")])