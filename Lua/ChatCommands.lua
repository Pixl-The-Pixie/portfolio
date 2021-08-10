local rf = RecipientFilter()
local PhysIgnitebool
local GetChatbool = true
local Echobool = true
spawnpoints = ents.FindByClass(navmesh.GetPlayerSpawnName())


/*note to self, add table or metatables for players to store their respective
command variables to make script more server friendly*/

function GetChat(ply, text, teambool)
  -- A simple function to log chat data to console.
  if GetChatbool == true then
    print("Steam Nickname")
    print( ply:Nick() )
    print("Steam ID")
    print( ply:AccountID() )
    print("Text Says")
    print(text)
    print("Team")
    print(ply:Team())
    print("Is team chat?")
    print(teambool)
  end

--Toggle chat echo for commands.
  if text == "+echo" && teambool then
    if Echobool == true then
      Echobool = false
      ply:ChatPrint("Echo has been disabled by" .. " " .. ply:Nick() .. ".")
    elseif Echobool == false then
      Echobool = true
      ply:ChatPrint("Echo has been enabled by" .. " " .. ply:Nick() .. ".")
    end
  end

--toggle chat log
  if text == "+chatlog" && teambool == true then
    if GetChatbool == true then
      GetChatbool = false
      print("Chat log disabled by " .. ply:Nick())
      if Echobool == true then
        ply:ChatPrint("Chat Log has been disabled by" .. " " .. ply:Nick() .. ".")
      end
    elseif GetChatbool == false then
      GetChatbool = true
      print("Chat log enabled by " .. ply:Nick())
      if Echobool == true then
        ply:ChatPrint("Chat Log has been enabled by " .. ply:Nick() .. ".")
      end
    end
  end

  --A simple chat based kill command.
  if text == "+kill" && teambool == true then
    ply:Kill()
    if Echobool == true then
      ply:ChatPrint(ply:Nick() .. "suicided.")
    end
  end

  --A command to get a table of team spawnclasses, and a list of spawnpoints
  if text == "+spawns" then
    local spawnclasses = team.GetSpawnPoint(ply:Team())
    local count = 0

    print("class")
    PrintTable(team.GetSpawnPoint(ply:Team()))
    print("enttable")
    PrintTable(team.GetSpawnPoints(ply:Team()))
    print("entities")
    PrintTable(ents.FindByClass(navmesh.GetPlayerSpawnName()))
    --Left in incase I want to implent it into something.
      for k, v in pairs(spawnpoints) do
        count = count + 1
      end
    print(count .. " total spawns detected.")
    if Echobool == true then
      ply:ChatPrint(count .. " total spawns detected.")
    end
  end
--A simple chat based status command to list the name and steamid3 of all players.
  if text == "+status" && teambool == true then
    rf:AddAllPlayers()

    ply:PrintMessage(2, "-------STATUS--------")
    for k, v in pairs(rf:GetPlayers()) do
      ply:PrintMessage(2, v:Nick() .. " : " .. v:AccountID())
    end

    local count = 0
    for k, v in pairs(rf:GetPlayers()) do
      count = count + 1
      ply:ChatPrint(v:Nick() .. " : " .. tostring(v:AccountID()))
    end
    ply:ChatPrint(tostring(count) .. " total players connected.")
  end

  --A toggle command for the PhysIgnite function.
  if text == "+physignite" && teambool == true then
    if PhysIgnitebool == true then
      PhysIgnitebool = false
      print("PhysGun Ignite disabled by " .. ply:Nick())
      if Echobool == true then
        ply:ChatPrint("PhysGun Ignite has been disabled by" .. " " .. ply:Nick() .. ".")
      end
    elseif PhysIgnitebool == false or PhysIgnitebool == nil then
      PhysIgnitebool = true
      print("PhysGun Ignite enabled by " .. ply:Nick())
      if Echobool == true then
        ply:ChatPrint("PhysGun Ignite has been enabled by " .. ply:Nick() .. ".")
      end
    end
  end
end

--A function that when active causes any prop picked up by a phys gun to be ignited.
function PhysIgnite(ply, ent)
  if PhysIgnitebool == true then
    ent:Ignite(100000)
  end
end

function OnSpawn(ply)
end

/*aknowledgement here that all chat based output is directed solely to the client using the command. Will likely change this for
a RecipientFilter at some point(although some commands will keep clientside only output, i.e the status command and the spawns
command)*/

--hooks for the phys ignite, getchat, and OnSpawn functions.
hook.Add("PlayerSay", "ChatRead", GetChat)
hook.Add("PhysgunPickup", "PhysPickup", PhysIgnite)
hook.Add("PlayerSpawn", "PlayerSpawn", OnSpawn)
