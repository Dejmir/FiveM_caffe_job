ESX              = nil

Citizen.CreateThread(function()
	while ESX == nil do
		TriggerEvent('esx:getSharedObject', function(obj) ESX = obj end)
		Citizen.Wait(0)
	end
end)

TriggerEvent('esx:getSharedObject', function(obj) ESX = obj end)

RegisterNetEvent('esx_extraitems:kawa_low')

AddEventHandler('esx_extraitems:kawa_low', function()
end)

RegisterNetEvent('esx_extraitems:kawa_medium')

AddEventHandler('esx_extraitems:kawa_medium', function()
end)

RegisterNetEvent('esx_extraitems:kawa_high')

AddEventHandler('esx_extraitems:kawa_high', function()
end)