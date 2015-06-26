<?php

require_once($_SERVER['DOCUMENT_ROOT']. "/phplib/comfunc.php");

/*********************************************************************/
function chkpass($teamid, $pwd){

	// Retrieve password

	$uq = "SELECT T.User_ID UID, U.Password Pwd FROM Teams T
		JOIN User U on T.User_ID = U.User_ID
		WHERE T.Team_ID = '$teamid' LIMIT 1";

	$ur = mysql_query($uq);

	if(mysql_num_rows($ur)){
		$urow = mysql_fetch_array($ur, MYSQL_ASSOC);
		if($pwd == $urow['Pwd'])
			return TRUE;
		else
			return TRUE; //  CHANGE TO FALSE BEFORE UPLOADING TO SITE
	}

	return TRUE; //  CHANGE TO FALSE BEFORE UPLOADING TO SITE
	
}  // end of function

function getTmOffset($lg){
	
	$list = chkList($lg);
	
	$lq = "SELECT MIN(Team_ID) ID FROM NHLTeam WHERE List='$list'";
	$lr = mysql_query($lq);
	
	return(mysql_result($lr,0));
	
}  // end of function

function getTeamAbv($teamid, $lg){
	
	$off = getTmOffset($lg);
	
	
	$id = $teamid + $off;  // add offset depending on Team List
	
	$tmq = "SELECT * FROM NHLTeam WHERE Team_ID='$id' LIMIT 1";
	$tmr = @mysql_query($tmq) or die("Could not retrieve team information.");
	
	$row = mysql_fetch_array($tmr, MYSQL_ASSOC);
	
	return $row['Abv'];

}  // end of function

function getAbv($teamid){
	
	$tmq = "SELECT Abv FROM Teams WHERE Team_ID='$teamid' LIMIT 1";
	$tmr = @mysql_query($tmq) or die("Could not retrieve team information.");
	
	$row = mysql_fetch_array($tmr, MYSQL_ASSOC);
	
	return $row['Abv'];
	
}  // end of function

function getUserID($teamid){
	
	$userq = "SELECT User_ID FROM Teams WHERE Team_ID='$teamid'";
	$userr = @mysql_query($userq) or die ("Could not retrieve User ID.");
	
	$row = mysql_fetch_array($userr, MYSQL_ASSOC);
	
	return $row['User_ID'];
	
}  // end of function

function getPlayerID($teamid, $offset, $list, $classic, $type){
	
	// Retrieve Abv first
	
	$abv = getAbv($teamid);

	// Retrieve Player_ID
	
	if($classic){
		$pq = "SELECT Player_ID FROM Roster WHERE League_ID='0' AND Abv='$abv' AND Type='$type' AND Offset='$offset' LIMIT 1";
	}
	else {
		$pq = "SELECT Player_ID FROM Roster WHERE Team_ID='$teamid' AND Offset='$offset' AND Status='N' LIMIT 1";
	}
	
/*	$pq = "SELECT Player_ID FROM NHLPlayer WHERE List='$list' AND Type='$type' AND Abv='$abv' AND Offset='$offset' AND Roster='N' LIMIT 1";
*/
	$pr = @mysql_query($pq) or die("Could not retrieve Player_ID.");
	
	$row = mysql_fetch_array($pr, MYSQL_ASSOC);
	
	return $row['Player_ID'];
	
	
}  // end of function

function errorcheck($teamid, $gameid, $pwd, $lg){
	
	$chk = chkpass($teamid, $pwd);
	$lgnm = lgname($lg);
	
	
	
	if($chk){	// Pass OK
		$scoreq = "SELECT S.Home Hm, S.Away Aw, S.Sub_League Sublg, H.User_ID HUD, H.Abv HAbv, A.User_ID AUD, A.Abv AAbv
			FROM Schedule S
			JOIN Teams H ON H.Team_ID = S.Home
			JOIN Teams A ON A.Team_ID = S.Away
			WHERE S.Game_ID = '$gameid' LIMIT 1";  // updated to include User_IDs
	
		$scorer = @mysql_query($scoreq) or die('ERROR 2004: Could not retrieve game information.');	
	

		if($scorer){
			$row = mysql_fetch_array($scorer, MYSQL_ASSOC);

			// Check type of league and check file
			
			$e = '.bad';
			
			if (substr($row['Sublg'], 0, 4) == "GENS"){
				$filetypes = array('.gs0','.gs1','.gs2','.gs3','.gs4','.gs5','.gs6','.gs7','.gs8','.gs9');
				$e = '.gs0';
			}
			else if (substr($row['Sublg'], 0, 4) == "SNES"){
				$filetypes = array('.zst','.zs1','.zs2','.zs3','.zs4','.zs5','.zs6','.zs7','.zs8','.zs9');	
				$e = '.zst';
			}
			
			$filename = $_FILES['uploadfile']['name']; // Get the name of the file (including file extension).
			$ext = substr($filename, strpos($filename,'.'), strlen($filename)-1); // Get the extension from the filename.
			$upload_path = $_SERVER['DOCUMENT_ROOT']. '/uploaded/'. $lg. '-'. $gameid. '.sv';
//			echo $upload_path;
			
			if(in_array($ext, $filetypes)){  // file ext is OK

				if(move_uploaded_file($_FILES['uploadfile']['tmp_name'], $upload_path)){
					
					// Check if teams are correct
					$fr = fopen("$upload_path", 'rb');	// reads file
					
					if (substr($row['Sublg'], 0, 4) == "GENS"){
						// Away Team
						fseek ($fr,59307);
						$StateAwayAbv = getTeamAbv(hexdec(bin2hex(fread($fr, 1))), $lg);

						// Home Team
						fseek ($fr,59305);
						$StateHomeAbv = getTeamAbv(hexdec(bin2hex(fread($fr, 1))), $lg);
						
//					echo $StateAwayAbv. 'vs. '. $StateHomeAbv;
//					die();
						
						if($StateHomeAbv == $row['HAbv'] && $StateAwayAbv == $row['AAbv'])  // Teams in state match schedule
							return 0;
						else
							return 1;  // teams do not match
					}
					
					else if (substr($row['Sublg'], 0, 4) == "SNES"){
						// Away Team
						fseek ($fr,10413);
						$StateAwayAbv = getTeamAbv(hexdec(bin2hex(fread($fr, 1))), $lg);

						// Home Team
						fseek ($fr,10411);
						$StateHomeAbv = getTeamAbv(hexdec(bin2hex(fread($fr, 1))), $lg);		
						if($StateHomeAbv == $row['HAbv'] && $StateAwayAbv == $row['AAbv'])  // Teams in state match schedule
							return 0;
						else
							return 1;  // teams do not match
					}
				
				}
					
				else
    				return 5;  // could not upload
			}
	
			else 
				return 3;  // file ext not correct
		}
	}
	
	else
		return 2;  // password not correct

} // end of function

function addgame($teamid, $gameid, $lg){	// add game to database
	
	$lgnm = lgname($lg);
	$file = $_SERVER['DOCUMENT_ROOT']. '/uploaded/'. $lg. '-'. $gameid. '.sv';  // game save
	
	$fr = fopen("$file", 'rb');	// reads file	
	
	$gmq = "SELECT * FROM Schedule WHERE Game_ID='$gameid' LIMIT 1";
	$gmr = @mysql_query($gmq) or die("Error:  Could not retrieve game data.");
	
	$row = mysql_fetch_array($gmr, MYSQL_ASSOC);
	
	// Has the game been uploaded while you were waiting?
	
	if($row['H_Confirm'] == 1 && $row['A_Confirm'] == 1)
		die("Error.  Game has been uploaded already.");
	
	// What type of save state will it be?  Check Sub_League for game
	
	if(substr($row['Sub_League'], 0, 4) == "GENS")
		$stattype = "GENS";
	else if (substr($row['Sub_League'], 0, 4) == "SNES")
		$stattype = "SNES";
	else
		die("Error: Problem with game data.  Please contact administrator.");
		
/**********************************************************************************/

		
	// Retrieve Coach User IDs
//	echo $gameid. " ". $row['Home']. " ". $row['Away'];
	$homeid = getUserID($row['Home']);
	$awayid = getUserID($row['Away']);
	$confid = getUserID($teamid);
	$sub = $row['Sub_League'];
	
	// Retrieve Blitz League Stats (some stats change)
	
	$blitz = blitzChk($lg);
	
//	echo $gameid. " ". $homeid. " ". $awayid;
	
	// Retreive Team List type
	
	$list = chkList($lg);
	
	if($stattype == "GENS"){  // Gens save state extraction
		
		// Crowd Meter
		fseek ($fr,59277);
   		$PeakMeter = hexdec(bin2hex(fread($fr, 1)));
		
		// AWAY TEAM STATS
		
		// Away Goals
		fseek ($fr,61111);
   		$AwayScore = hexdec(bin2hex(fread($fr, 1)));
		
		// Away Power Play Goals/Opportunities
		fseek ($fr,61101);
 		$AwayPPG = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,61103);
 		$AwayPP = hexdec(bin2hex(fread($fr, 1)));
		
		// Away SHG
		fseek ($fr,61953);
 		$AwaySHG = hexdec(bin2hex(fread($fr, 1)));

		// Away SHGA
		fseek ($fr,61085);
 		$AwaySHGA = hexdec(bin2hex(fread($fr, 1))); 
	
		// Away Breakaway Goals/Opportunities 
		fseek ($fr,61957);
 		$AwayBKG = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,61955);
 		$AwayBKO = hexdec(bin2hex(fread($fr, 1)));
	
		// Away One Timer Goals:Attempts
		fseek ($fr,61961);
		$Away1TG = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,61959);
 		$Away1TA = hexdec(bin2hex(fread($fr, 1)));
	
		// Away Penalty Shot Goals:Attempts
		fseek ($fr,61965);
 		$AwayPSG = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,61963);
   		$AwayPSA = hexdec(bin2hex(fread($fr, 1)));

		// Away Faceoffs Won
		fseek ($fr,61113);
 		$AwayFOW = hexdec(bin2hex(fread($fr, 1)));
	
		// Away Body Checks
		fseek ($fr,61115);
 		$AwayByCks = hexdec(bin2hex(fread($fr, 1)));
	
		// Away Team Penalties / Minutes
		fseek ($fr,61105);
 		$AwayPen = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,61107);
 		$AwayPenM = hexdec(bin2hex(fread($fr, 1)));
	
		// Away Attack Zone
		fseek ($fr,61108);
 		$AwayAZMinutes = hexdec(bin2hex(fread($fr, 1))) *256;
		fseek ($fr,61109);
 		$AwayAZSeconds = hexdec(bin2hex(fread($fr, 1)));
 		$AwayAZ = ($AwayAZMinutes + $AwayAZSeconds);
 		$AwayAZDisplayM = Round((Floor($AwayAZ/60)),0);
 		$AwayAZDisplayS = ($AwayAZ % 60);
	
		// Away Passing Stats
		fseek ($fr,61119);
 		$AwayPsC = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,61117);
 		$AwayPsA = hexdec(bin2hex(fread($fr, 1)));
		
		// HOME TEAM STATS
		
		// Home Goals
		fseek ($fr,60243);
   		$HomeScore = hexdec(bin2hex(fread($fr, 1)));
		
		// Home Power Play Goals/Opportunities
		fseek ($fr,60233);
 		$HomePPG = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,60235);
   		$HomePP = hexdec(bin2hex(fread($fr, 1)));
	
		// Home SHG
		fseek ($fr,61085);
 		$HomeSHG = hexdec(bin2hex(fread($fr, 1)));
	
		// Home SHGA
		fseek ($fr,61953);
   		$HomeSHGA = hexdec(bin2hex(fread($fr, 1)));
	
		// Home Breakaway Goals/Opportunities 
		fseek ($fr,61089);
   		$HomeBKG = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,61087);
   		$HomeBKO = hexdec(bin2hex(fread($fr, 1)));
	
		// Home One Timer Goals:Attempts
		fseek ($fr,61093);
   		$Home1TG = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,61091);
   		$Home1TA = hexdec(bin2hex(fread($fr, 1)));
	
		// Home Penalty Shot Goals:Attempts
		fseek ($fr,61097);
   		$HomePSG = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,61095);
   		$HomePSA = hexdec(bin2hex(fread($fr, 1)));
	
		// Home Faceoffs Won 
		fseek ($fr,60245);
   		$HomeFOW = hexdec(bin2hex(fread($fr, 1)));
	
		// Home Body Checks
		fseek ($fr,60247);
   		$HomeByCks = hexdec(bin2hex(fread($fr, 1)));
	
		// Home Team Penalties / Minutes
		fseek ($fr,60237);
   		$HomePen = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,60239);
   		$HomePenM = hexdec(bin2hex(fread($fr, 1)));
	
		// Home Attack Zone
		fseek ($fr,60240);
   		$HomeAZMinutes = hexdec(bin2hex(fread($fr, 1))) *256;
		fseek ($fr,60241);
   		$HomeAZSeconds = hexdec(bin2hex(fread($fr, 1)));
   		$HomeAZ = ($HomeAZMinutes + $HomeAZSeconds);
   		$HomeAZDisplayM = Round((Floor($HomeAZ/60)),0);
   		$HomeAZDisplayS = ($HomeAZ % 60);
	
		// Home Passing Stats
		fseek ($fr,60251);
   		$HomePsC = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,60249);
   		$HomePsA = hexdec(bin2hex(fread($fr, 1)));
		
		/*********PERIOD STATS*************/
		
		// Away Team Period Goals
		
		fseek ($fr,61933);
   		$A1stGoals = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,61935);
   		$A2ndGoals = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,61937);
   		$A3rdGoals = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,61939);
   		$AOTGoals = hexdec(bin2hex(fread($fr, 1)));
	
		// Away Team Period SOG
		fseek ($fr,61941);
   		$A1stSOG = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,61943);
   		$A2ndSOG = hexdec(bin2hex(fread($fr, 1)));

		fseek ($fr,61945);
   		$A3rdSOG = hexdec(bin2hex(fread($fr, 1)));

		fseek ($fr,61947);
   		$AOTSOG = hexdec(bin2hex(fread($fr, 1)));
	
		//--------- End of Away Period Stats---------------

		//--------- Home Team Period Stats-----------------

		// Home Team Period Goals
		fseek ($fr,61065);
   		$H1stGoals = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,61067);
   		$H2ndGoals = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,61069);
   		$H3rdGoals = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,61071);
   		$HOTGoals = hexdec(bin2hex(fread($fr, 1)));
	
		// Home Team Period SOG
		fseek ($fr,61073);
   		$H1stSOG = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,61075);
   		$H2ndSOG = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,61077);
   		$H3rdSOG = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,61079);
   		$HOTSOG = hexdec(bin2hex(fread($fr, 1)));
	
		//------ End of Home Period Stats ---------
		
	}  // end of GENS stats

	else if ($stattype == "SNES"){
		
		// Crowd Meter
		fseek ($fr,9687);
   		$PeakMeter = hexdec(bin2hex(fread($fr, 1)));
		
		// AWAY TEAM STATS
		
		// Away Goals
		fseek ($fr,9123);
   		$AwayScore = hexdec(bin2hex(fread($fr, 1)));
		
		// Away Power Play Goals/Opportunities
		fseek ($fr,9047);
 		$AwayPPG = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,9051);
 		$AwayPP = hexdec(bin2hex(fread($fr, 1)));
		
		// Away SHG
		fseek ($fr,9079);
 		$AwaySHG = hexdec(bin2hex(fread($fr, 1)));

		// Away SHGA
		fseek ($fr,9077);
 		$AwaySHGA = hexdec(bin2hex(fread($fr, 1))); 
	
		// Away Breakaway Goals/Opportunities 
		fseek ($fr,9179);
 		$AwayBKG = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,9175);
 		$AwayBKO = hexdec(bin2hex(fread($fr, 1)));
	
		// Away One Timer Goals:Attempts
		fseek ($fr,9199);
		$Away1TG = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,9195);
 		$Away1TA = hexdec(bin2hex(fread($fr, 1)));
	
		// Away Penalty Shot Goals:Attempts
		fseek ($fr,9187);
 		$AwayPSG = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,9183);
   		$AwayPSA = hexdec(bin2hex(fread($fr, 1)));

		// Away Faceoffs Won
		fseek ($fr,9127);
 		$AwayFOW = hexdec(bin2hex(fread($fr, 1)));
	
		// Away Body Checks
		fseek ($fr,9163);
 		$AwayByCks = hexdec(bin2hex(fread($fr, 1)));
	
		// Away Team Penalties / Minutes
		fseek ($fr,9055);
 		$AwayPen = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,9059);
 		$AwayPenM = hexdec(bin2hex(fread($fr, 1)));
	
		// Away Attack Zone
		fseek ($fr,9064);
 		$AwayAZMinutes = hexdec(bin2hex(fread($fr, 1))) *256;
		fseek ($fr,9063);
 		$AwayAZSeconds = hexdec(bin2hex(fread($fr, 1)));
 		$AwayAZ = ($AwayAZMinutes + $AwayAZSeconds);
 		$AwayAZDisplayM = Round((Floor($AwayAZ/60)),0);
 		$AwayAZDisplayS = ($AwayAZ % 60);
	
		// Away Passing Stats
		fseek ($fr,9167);
 		$AwayPsC = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,9171);
 		$AwayPsA = hexdec(bin2hex(fread($fr, 1)));
		
		// HOME TEAM STATS
		
		// Home Goals
		fseek ($fr,9121);
   		$HomeScore = hexdec(bin2hex(fread($fr, 1)));
		
		// Home Power Play Goals/Opportunities
		fseek ($fr,9045);
 		$HomePPG = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,9049);
   		$HomePP = hexdec(bin2hex(fread($fr, 1)));
	
		// Home SHG
		fseek ($fr,9077);
 		$HomeSHG = hexdec(bin2hex(fread($fr, 1)));
	
		// Home SHGA
		fseek ($fr,9079);
   		$HomeSHGA = hexdec(bin2hex(fread($fr, 1)));
	
		// Home Breakaway Goals/Opportunities 
		fseek ($fr,9177);
   		$HomeBKG = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,9173);
   		$HomeBKO = hexdec(bin2hex(fread($fr, 1)));
	
		// Home One Timer Goals:Attempts
		fseek ($fr,9197);
   		$Home1TG = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,9193);
   		$Home1TA = hexdec(bin2hex(fread($fr, 1)));
	
		// Home Penalty Shot Goals:Attempts
		fseek ($fr,9185);
   		$HomePSG = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,9181);
   		$HomePSA = hexdec(bin2hex(fread($fr, 1)));
	
		// Home Faceoffs Won 
		fseek ($fr,9125);
   		$HomeFOW = hexdec(bin2hex(fread($fr, 1)));
	
		// Home Body Checks
		fseek ($fr,9161);
   		$HomeByCks = hexdec(bin2hex(fread($fr, 1)));
	
		// Home Team Penalties / Minutes
		fseek ($fr,9053);
   		$HomePen = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,9057);
   		$HomePenM = hexdec(bin2hex(fread($fr, 1)));
	
		// Home Attack Zone
		fseek ($fr,9062);
   		$HomeAZMinutes = hexdec(bin2hex(fread($fr, 1))) *256;
		fseek ($fr,9061);
   		$HomeAZSeconds = hexdec(bin2hex(fread($fr, 1)));
   		$HomeAZ = ($HomeAZMinutes + $HomeAZSeconds);
   		$HomeAZDisplayM = Round((Floor($HomeAZ/60)),0);
   		$HomeAZDisplayS = ($HomeAZ % 60);
	
		// Home Passing Stats
		fseek ($fr,9165);
   		$HomePsC = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,9169);
   		$HomePsA = hexdec(bin2hex(fread($fr, 1)));
		
		/*********PERIOD STATS*************/
		
		// Away Team Period Goals
		
		fseek ($fr,9107);
   		$A1stGoals = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,9111);
   		$A2ndGoals = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,9115);
   		$A3rdGoals = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,9119);
   		$AOTGoals = hexdec(bin2hex(fread($fr, 1)));
	
		// Away Team Period SOG
		fseek ($fr,9087);
   		$A1stSOG = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,9091);
   		$A2ndSOG = hexdec(bin2hex(fread($fr, 1)));

		fseek ($fr,9095);
   		$A3rdSOG = hexdec(bin2hex(fread($fr, 1)));

		fseek ($fr,9099);
   		$AOTSOG = hexdec(bin2hex(fread($fr, 1)));
	
		//--------- End of Away Period Stats---------------

		//--------- Home Team Period Stats-----------------

		// Home Team Period Goals
		fseek ($fr,9105);
   		$H1stGoals = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,9109);
   		$H2ndGoals = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,9113);
   		$H3rdGoals = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,9117);
   		$HOTGoals = hexdec(bin2hex(fread($fr, 1)));
	
		// Home Team Period SOG
		fseek ($fr,9085);
   		$H1stSOG = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,9089);
   		$H2ndSOG = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,9093);
   		$H3rdSOG = hexdec(bin2hex(fread($fr, 1)));
	
		fseek ($fr,9097);
   		$HOTSOG = hexdec(bin2hex(fread($fr, 1)));
	
		//------ End of Home Period Stats ---------
		
	
	}
		
	// OT Game?
	
	$HTotalGoals = $H1stGoals + $H2ndGoals + $H3rdGoals + $HOTGoals;
	$ATotalGoals = $A1stGoals + $A2ndGoals + $A3rdGoals + $AOTGoals;
	
	if(($HOTSOG > 0) || ($AOTSOG > 0) || ($HOTGoals > 0) || ($AOTGoals > 0) || ($HTotalGoals == $ATotalGoals))  // OT Game
		$OT = '1';
	else
		$OT = '0';
			
	// Update Schedule
	
	
	$schupq = "UPDATE Schedule SET H_User_ID = '$homeid', A_User_ID = '$awayid', H_Score = '$HomeScore', A_Score = '$AwayScore',
				OT = '$OT', H_Confirm = '1', A_Confirm = '1', Confirm_Time = NOW() WHERE Game_ID='$gameid' LIMIT 1";
	$schupr = @mysql_query($schupq);
	
	if(!$schupr)  // update failed
		return 4;
		
	// Add to GameStats
	
	$hazstring = '00:'. $HomeAZDisplayM. ':'. $HomeAZDisplayS;		// Attack Zone String
	$aazstring = '00:'. $AwayAZDisplayM. ':'. $AwayAZDisplayS;
	
	$goalf = ", GHP1, GHP2, GHP3, GHOT, GAP1, GAP2, GAP3, GAOT";
	$goalv = ", '$H1stGoals', '$H2ndGoals', '$H3rdGoals', '$HOTGoals', '$A1stGoals', '$A2ndGoals', '$A3rdGoals', '$AOTGoals'";
	$shotf = ", SHP1, SHP2, SHP3, SHOT, SAP1, SAP2, SAP3, SAOT";
	$shotv = ", '$H1stSOG', '$H2ndSOG', '$H3rdSOG', '$HOTSOG', '$A1stSOG', '$A2ndSOG', '$A3rdSOG', '$AOTSOG'"; 
	$pmf = ", PIMH, PIMA";
	$pmv = ", '$HomePenM', '$AwayPenM'";
	$bcf = ", BCH, BCA";
	$bcv = ", '$HomeByCks', '$AwayByCks'";
	$ppshf = ", PPHG, PPAG, SHHG, SHAG, PPH, PPA";
	$ppshv = ", '$HomePPG', '$AwayPPG', '$HomeSHG', '$AwaySHG', '$HomePP', '$AwayPP'";
	$baf = ", BAHG, BAAG, BAH, BAA";
	$bav = ", '$HomeBKG', '$AwayBKG', '$HomeBKO', '$AwayBKO'";
	$otf = ", 1THG, 1TAG, 1TH, 1TA";
	$otv = ", '$Home1TG', '$Away1TG', '$Home1TA', '$Away1TA'";
	$psf = ", PSHG, PSAG, PSH, PSA";
	$psv = ", '$HomePSG', '$AwayPSG', '$HomePSA', '$AwayPSA'";
	$foakf = ", FOH, FOA, AZH, AZA";
	$foakv = ", '$HomeFOW', '$AwayFOW', '$hazstring', '$aazstring'";
	$pasf = ", PCH, PH, PCA, PA";
	$pasv = ", '$HomePsC', '$HomePsA', '$AwayPsC', '$AwayPsA'";
	
	$gsq = "INSERT INTO GameStats (Game_ID, League_ID, Sub_League, ConUser_ID, Crowd
			$goalf $shotf $pmf $bcf $ppshf $baf $otf $psf $foakf $pasf)
			VALUES ('$gameid', '$lg', '$sub', '$confid', '$PeakMeter' $goalv $shotv $pmv $bcv $ppshv $bav $otv $psv $foakv $pasv)";
	//echo $gsq;
	$gsr = @mysql_query($gsq);
	
	if(!$gsr)  // insert failed
		return 5;
		
/**********************************************************************************/

		
	// Add Player Stats
	
	// Retrieve Team Abvs and Team_IDs for Home and Away
	
	$hm = getAbv($row['Home']);
	$hmtm = $row['Home'];
	$aw = getAbv($row['Away']);
	$awtm = $row['Away'];
	$i = 1;
	
	// Check for Classic League
	
	$classic = classicChk($lg);
	
	
	if($stattype == "GENS"){	// GENS Player Stats
	
		// Home Team
	
		if($classic){
			$plq = "SELECT Player_ID, Abv, Pos FROM Roster WHERE League_ID='0' AND Type='G' AND Abv='$hm' ORDER BY Offset ASC";
		}
		else {
			$plq = "SELECT Player_ID, Abv, Pos FROM Roster WHERE League_ID='$lg' AND Team_ID='$hmtm' AND Status='N' ORDER BY Offset ASC";
		}
		
/*		
		$plq = "SELECT Player_ID, Type, Abv, Pos FROM NHLPlayer WHERE Type='GENS' AND List='$list' AND Abv='$hm' AND Roster='N' ORDER BY Offset ASC";
*/
		$plr = @mysql_query($plq) or die("Could not retrieve Home Player List.  Please contact administrator.");
		//echo $plq;
		
		while($prow = mysql_fetch_array($plr, MYSQL_ASSOC)){	// Team Roster
		
			// Home Player Stats
			
			$pid = $prow['Player_ID'];
			$pos = $prow['Pos'];
		
			fseek ($fr,60409 + $i);
   			$Goals = hexdec(bin2hex(fread($fr, 1)));

			fseek ($fr,60435 + $i);
   			$Assists = hexdec(bin2hex(fread($fr, 1)));
			$Points = $Goals + $Assists;

			fseek ($fr,60461 + $i);
   			$SOG = hexdec(bin2hex(fread($fr, 1)));

			if($blitz == 1){  // Blitz League
				
				fseek ($fr,60487 + $i);		// In Blitz, this is Chks For
   				$Chksfor = hexdec(bin2hex(fread($fr, 1)));
			
				fseek ($fr,60513 + $i);		// In Blitz, this is Chks Against
   				$ChksA = hexdec(bin2hex(fread($fr, 1)));
				
				$PIM = 0;  // will be calculated later
				$PlusMinus = 0;
			
			}		
			else {
			
				fseek ($fr,60487 + $i);		
   				$PIM = hexdec(bin2hex(fread($fr, 1)));

				fseek ($fr,60513 + $i);		
   				$Chksfor = hexdec(bin2hex(fread($fr, 1)));

				$ChksA = 0;
				$PlusMinus = 0;
			
			}

			// Home TOI
			fseek ($fr,(60538 + ($i * 2)));
   			$TOIMinutes = hexdec(bin2hex(fread($fr, 1))) * 256;
			fseek ($fr,(60539 + ($i * 2)));
   			$TOISeconds = hexdec(bin2hex(fread($fr, 1)));
   			
			$TOI = ($TOIMinutes + $TOISeconds);
   			$TOIDisplayM = Round((Floor($TOI/60)),0);
   			$TOIDisplayS = ($TOI % 60);
			
			// Compensate for TOI bug in Genesis
			
			$TOIDisplayS = $TOIDisplayS - 2;  
			if($TOIDisplayM == 0 && $TOIDisplayS == 0)
				$TOIDisplayS == 2;
				
			$TOIString = "00:". $TOIDisplayM. ":". $TOIDisplayS;
			
			if($Goals != '0' || $Assists != '0' || $SOG != '0' || $PIM != '0' || $Chksfor != '0' || $TOI != '0'){  // Played Game 
				
				// Insert Stats into PlayerStats
				
				$psq = "INSERT INTO PlayerStats (Game_ID, League_ID, Sub_League, Team_ID, Player_ID, Pos, G, A, SOG, PIM, Chks, TOI, ChksA, PlusMinus)
						VALUES ('$gameid', '$lg', '$sub', '$hmtm', '$pid', '$pos', '$Goals', '$Assists', '$SOG', '$PIM',
						'$Chksfor', '$TOIString', $ChksA, $PlusMinus)";
				$psr = @mysql_query($psq) or die("Could not enter Player Stats.  Please contact administrator.");
				
			}
		
		$i++;
		}
		
		// Away Team
		
		$i = 1;
		
		if($classic){
			$plq = "SELECT Player_ID, Abv, Pos FROM Roster WHERE League_ID='0' AND Type='G' AND Abv='$aw' ORDER BY Offset ASC";
		}
		else {
			$plq = "SELECT Player_ID, Abv, Pos FROM Roster WHERE League_ID='$lg' AND Team_ID='$awtm' AND Status='N' ORDER BY Offset ASC";
		}
		
/*		$plq = "SELECT Player_ID, Type, Abv, Pos FROM NHLPlayer WHERE Type='GENS' AND List='$list' AND Abv='$aw' AND Roster='N' ORDER BY Offset ASC";
*/
		$plr = @mysql_query($plq) or die("Could not retrieve Away Player List.  Please contact administrator.");
		
		while($prow = mysql_fetch_array($plr, MYSQL_ASSOC)){  // Team Roster
		
			// Away Player Stats
			
			$pid = $prow['Player_ID'];
			$pos = $prow['Pos'];
			
			fseek ($fr,61277 + $i);
   			$Goals = hexdec(bin2hex(fread($fr, 1)));

			fseek ($fr,61303 + $i);
   			$Assists = hexdec(bin2hex(fread($fr, 1)));
   			$Points = $Goals + $Assists;

			fseek ($fr,61329 + $i);
   			$SOG = hexdec(bin2hex(fread($fr, 1)));

			if($blitz == 1){  // Blitz League
				
				fseek ($fr,61355 + $i);		// In Blitz, this is Chks For
   				$Chksfor = hexdec(bin2hex(fread($fr, 1)));
			
				fseek ($fr,61381 + $i);		// In Blitz, this is Chks Against
   				$ChksA = hexdec(bin2hex(fread($fr, 1)));
				
				$PIM = 0;  // will be calculated later
				$PlusMinus = 0;
			
			}		
			else {

				fseek ($fr,61355 + $i);
   				$PIM = hexdec(bin2hex(fread($fr, 1)));

				fseek ($fr,61381 + $i);
   				$Chksfor = hexdec(bin2hex(fread($fr, 1)));
			
				$ChksA = 0;
				$PlusMinus = 0;

			}

			// Away TOI
			fseek ($fr,(61406 + ($i * 2)));
   			$TOIMinutes = hexdec(bin2hex(fread($fr, 1))) * 256;
			fseek ($fr,(61407 + ($i * 2)));
   			$TOISeconds = hexdec(bin2hex(fread($fr, 1)));
   			$TOI = ($TOIMinutes + $TOISeconds);
   			$TOIDisplayM = Round((Floor($TOI/60)),0);
   			$TOIDisplayS = ($TOI % 60);
			
			// Compensate for TOI bug in Genesis
			
			$TOIDisplayS = $TOIDisplayS - 2;  
			if($TOIDisplayM == 0 && $TOIDisplayS == 0)
				$TOIDisplayS == 2;
				
			$TOIString = "00:". $TOIDisplayM. ":". $TOIDisplayS;
			
			if($Goals != '0' || $Assists != '0' || $SOG != '0' || $PIM != '0' || $Chksfor != '0' || $TOI != '0'){  // Played Game 
				
				// Insert Stats into PlayerStats
				
				$psq = "INSERT INTO PlayerStats (Game_ID, League_ID, Sub_League, Team_ID, Player_ID, Pos, G, A, SOG, PIM, Chks, TOI, ChksA, PlusMinus)
						VALUES ('$gameid', '$lg', '$sub', '$awtm', '$pid', '$pos', '$Goals', '$Assists', '$SOG', '$PIM',
						'$Chksfor', '$TOIString', $ChksA, $PlusMinus)";
				$psr = @mysql_query($psq) or die("Could not enter Player Stats.  Please contact administrator.");
				
			}
		
		$i++;
		}
		
	}
	
	else if($stattype == "SNES"){	// SNES Player Stats
		
			// Home Team
		
		if($classic){
			$plq = "SELECT Player_ID, Abv, Pos FROM Roster WHERE League_ID='0' AND Type='S' AND Abv='$hm' ORDER BY Offset ASC";
		}
		else {
			$plq = "SELECT Player_ID, Abv, Pos FROM Roster WHERE League_ID='$lg' AND Team_ID='$hmtm' AND Status='N' ORDER BY Offset ASC";
		}
		
/*		$plq = "SELECT Player_ID, Type, Abv, Pos FROM NHLPlayer WHERE Type='SNES' AND List='$list' AND Abv='$hm' AND Roster='N' ORDER BY Offset ASC";
*/		
		$plr = @mysql_query($plq) or die("Could not retrieve Home Player List.  Please contact administrator.");
		//echo $plq;
		
		while($prow = mysql_fetch_array($plr, MYSQL_ASSOC)){	// Team Roster
		
			// Home Player Stats
			
			$pid = $prow['Player_ID'];
			$pos = $prow['Pos'];
		
			fseek ($fr,9472 + $i);
   			$Goals = hexdec(bin2hex(fread($fr, 1)));

			fseek ($fr,9524 + $i);
   			$Assists = hexdec(bin2hex(fread($fr, 1)));
			$Points = $Goals + $Assists;

			fseek ($fr,9576 + $i);
   			$SOG = hexdec(bin2hex(fread($fr, 1)));

			fseek ($fr,9628 + $i);
   			$PIM = hexdec(bin2hex(fread($fr, 1)));
			
			/*  Currently, cannot find Chks For or TOI in SNES */

//			fseek ($fr,60513 + $i);
//  		$Chksfor = hexdec(bin2hex(fread($fr, 1)));

			// Home TOI
/*			fseek ($fr,(60538 + ($i * 2)));
   			$TOIMinutes = hexdec(bin2hex(fread($fr, 1))) * 256;
			fseek ($fr,(60539 + ($i * 2)));
   			$TOISeconds = hexdec(bin2hex(fread($fr, 1)));
   			
			$TOI = ($TOIMinutes + $TOISeconds);
   			$TOIDisplayM = Round((Floor($TOI/60)),0);
   			$TOIDisplayS = ($TOI % 60);
			$TOIString = "00:". $TOIDisplayM. ":". $TOIDisplayS;
*/			

			$Chksfor = "0";
			$TOIString = "00:00:00";
			
			if($Goals != '0' || $Assists != '0' || $SOG != '0' || $PIM != '0' || $Chksfor != '0' || $TOIString != '00:00:00'){  
			// Played Game 
				
				// Insert Stats into PlayerStats
				
				$psq = "INSERT INTO PlayerStats (Game_ID, League_ID, Sub_League, Team_ID, Player_ID, Pos, G, A, SOG, PIM,Chks, TOI)
						VALUES ('$gameid', '$lg', '$sub', '$hmtm', '$pid', '$pos', '$Goals', '$Assists', '$SOG', '$PIM',
						'$Chksfor', '$TOIString')";
				$psr = @mysql_query($psq) or die("Could not enter Player Stats.  Please contact administrator.");
				
			}
		
		$i++;
		}
		
		// Away Team
		
		$i = 1;
		
		if($classic){
			$plq = "SELECT Player_ID, Abv, Pos FROM Roster WHERE League_ID='0' AND Type='S' AND Abv='$aw' ORDER BY Offset ASC";
		}
		else {
			$plq = "SELECT Player_ID, Abv, Pos FROM Roster WHERE League_ID='$lg' AND Team_ID='$awtm' AND Status='N' ORDER BY Offset ASC";
		}
		
/*		$plq = "SELECT Player_ID, Type, Abv, Pos FROM NHLPlayer WHERE Type='SNES' AND List='$list' AND Abv='$aw' AND Roster='N' ORDER BY Offset ASC";
*/
		$plr = @mysql_query($plq) or die("Could not retrieve Away Player List.  Please contact administrator.");
		
		while($prow = mysql_fetch_array($plr, MYSQL_ASSOC)){  // Team Roster
		
			// Away Player Stats
			
			$pid = $prow['Player_ID'];
			$pos = $prow['Pos'];
			
			fseek ($fr,9498 + $i);
   			$Goals = hexdec(bin2hex(fread($fr, 1)));

			fseek ($fr,9550 + $i);
   			$Assists = hexdec(bin2hex(fread($fr, 1)));
   			$Points = $Goals + $Assists;

			fseek ($fr,9602 + $i);
   			$SOG = hexdec(bin2hex(fread($fr, 1)));

			fseek ($fr,9654 + $i);
   			$PIM = hexdec(bin2hex(fread($fr, 1)));

//			fseek ($fr,61381 + $i);
//   		$Chksfor = hexdec(bin2hex(fread($fr, 1)));

			// Away TOI
/*			fseek ($fr,(61406 + ($i * 2)));
   			$TOIMinutes = hexdec(bin2hex(fread($fr, 1))) * 256;
			fseek ($fr,(61407 + ($i * 2)));
   			$TOISeconds = hexdec(bin2hex(fread($fr, 1)));
   			$TOI = ($TOIMinutes + $TOISeconds);
   			$TOIDisplayM = Round((Floor($TOI/60)),0);
   			$TOIDisplayS = ($TOI % 60);
			$TOIString = "00:". $TOIDisplayM. ":". $TOIDisplayS;
*/			
			$Chksfor = 0;
			$TOIString = "00:00:00";
				
			if($Goals != '0' || $Assists != '0' || $SOG != '0' || $PIM != '0' || $Chksfor != '0' || $TOIString != '00:00:00'){  
			// Played Game 
				
				// Insert Stats into PlayerStats
				
				$psq = "INSERT INTO PlayerStats (Game_ID, League_ID, Sub_League, Team_ID, Player_ID, Pos, G, A, SOG, PIM,Chks, TOI)
						VALUES ('$gameid', '$lg', '$sub', '$awtm', '$pid', '$pos', '$Goals', '$Assists', '$SOG', '$PIM',
						'$Chksfor', '$TOIString')";
				$psr = @mysql_query($psq) or die("Could not enter Player Stats.  Please contact administrator.");
				
			}
		
		$i++;
		}
		
		
	}
	
/**********************************************************************************/

	
	// Scoring Summary
	
	if($stattype == "GENS"){
	
		$tmpExtract = 59627;	// Scoring Summary Length Offset
		fseek ($fr,59627);
  	 	$EndofSS = hexdec(bin2hex(fread($fr, 1)));	
		
		for ($i=1;$i<(($EndofSS + 6) / 6);$i+=1){
			
			// Period of Goal
			fseek ($fr,$tmpExtract + 1);
 			$Period = (int) (hexdec(bin2hex(fread($fr, 1))) / 64) + 1;
	
			// Time of Goal (in Seconds)

			fseek ($fr,$tmpExtract + 1);
	 		$GoalSec =  (hexdec(bin2hex(fread($fr, 1))));
			fseek ($fr,$tmpExtract + 2);
 			$Goaltmp =  (hexdec(bin2hex(fread($fr, 1))));
			$GoalTime = ($GoalSec * 256) + $Goaltmp;
			$GoalSec = $GoalSec * 256 + $Goaltmp - ($Period - 1) * 16384;
			$GoalMin = (int) ($GoalSec / 60);
			$GoalSec = ($GoalSec % 60);
			if($GoalSec < 10)
				$Sec = '0'. $GoalSec;
			else
				$Sec = $GoalSec;
			$time = $GoalMin. ":". $Sec;
			
			// Team that scored, type of goal
			
			fseek ($fr,$tmpExtract + 3);
	 		$GoalTeam = (bin2hex(fread($fr, 1)));
			switch($GoalTeam){
			
				case('0'):
					$team = $hmtm;
					$type = 'SH2';
				break;
				case('1'):
					$team = $hmtm;
					$type = 'SH';
				break;
				case('2'):
					$team = $hmtm;
					$type = 'EV';
				break;
				case('3'):
					$team = $hmtm;
					$type = 'PP';
				break;
				case('4'):
					$team = $hmtm;
					$type = 'PP2';
				break;
				case('80'):
					$team = $awtm;
					$type = 'SH2';
				break;
				case('81'):
					$team = $awtm;
					$type = 'SH';
				break;
				case('82'):
					$team = $awtm;
					$type = 'EV';
				break;
				case('83'):
					$team = $awtm;
					$type = 'PP';
				break;
				case('84'):
					$team = $awtm;
					$type = 'PP2';
				break;	
				default:
					die("Error with Scoring Summary. Could not retrieve Scoring Team Info.");
				break;
			}
			
			// Player that scored
			fseek ($fr,$tmpExtract + 4);
 			$GoalPlayer = (hexdec(bin2hex(fread($fr, 1))));
			$goalid = getPlayerID($team, $GoalPlayer, $stattype, $classic, 'G');
			
			// Assisters on Goal
			fseek ($fr,$tmpExtract + 5);
 			$GoalAst1 = (hexdec(bin2hex(fread($fr, 1))));
			if($GoalAst1 != 255)  // Assist occurred
				$a1id = getPlayerID($team, $GoalAst1, $stattype, $classic, 'G');
			else
				$a1id = 0;
			
			fseek ($fr,$tmpExtract + 6);
 			$GoalAst2 = (hexdec(bin2hex(fread($fr, 1))));
			if($GoalAst2 != 255)  // Assist occurred
				$a2id = getPlayerID($team, $GoalAst2, $stattype, $classic, 'G');
			else
				$a2id = 0;

			// Enter Scoring Summary into database
			
			$ssq = "INSERT INTO ScoreSum (Game_ID, League_ID, Sub_League, Team_ID, Period, Time, G, A1, A2, Type)
					VALUES ('$gameid', '$lg', '$sub', '$team', '$Period', '$time', '$goalid', '$a1id', '$a2id', '$type')";
			$ssr = @mysql_query($ssq) or die("Could not enter Score Summary.");
			
			$tmpExtract = ($tmpExtract + 6);  // move to next goal summary
		
		}
	}
	
	else if($stattype == "SNES"){
		
		$tmpExtract = 15694;	// Scoring Summary Length Offset
		fseek ($fr, 15693);
  	 	$last = hexdec(bin2hex(fread($fr, 1)));
		fseek ($fr, 15694);
		$first = hexdec(bin2hex(fread($fr, 1)));
		$EndofSS = ($first * 256) + $last;	
		
		for ($i=1;$i<(($EndofSS + 6) / 6);$i+=1){
			
			// Period of Goal
			fseek ($fr,$tmpExtract + 2);
 			$Period = (int) (hexdec(bin2hex(fread($fr, 1))) / 64) + 1;
	
			// Time of Goal (in Seconds)

			fseek ($fr,$tmpExtract + 2);
	 		$GoalSec =  (hexdec(bin2hex(fread($fr, 1))));
			fseek ($fr,$tmpExtract + 1);
 			$Goaltmp =  (hexdec(bin2hex(fread($fr, 1))));
			$GoalTime = ($GoalSec * 256) + $Goaltmp;
			$GoalSec = $GoalSec * 256 + $Goaltmp - ($Period - 1) * 16384;
			$GoalMin = (int) ($GoalSec / 60);
			$GoalSec = ($GoalSec % 60);
			if($GoalSec < 10)
				$Sec = '0'. $GoalSec;
			else
				$Sec = $GoalSec;
			$time = $GoalMin. ":". $Sec;
			
			// Team that scored, type of goal
			
			fseek ($fr,$tmpExtract + 3);
	 		$GoalTeam = (bin2hex(fread($fr, 1)));
//			echo $GoalTeam. '-';
			switch($GoalTeam){
			
				case('0'):
					$team = $hmtm;
					$type = 'SH2';
				break;
				case('1'):
					$team = $hmtm;
					$type = 'SH';
				break;
				case('2'):
					$team = $hmtm;
					$type = 'EV';
				break;
				case('3'):
					$team = $hmtm;
					$type = 'PP';
				break;
				case('4'):
					$team = $hmtm;
					$type = 'PP2';
				break;
				case('80'):
					$team = $awtm;
					$type = 'SH2';
				break;
				case('81'):
					$team = $awtm;
					$type = 'SH';
				break;
				case('82'):
					$team = $awtm;
					$type = 'EV';
				break;
				case('83'):
					$team = $awtm;
					$type = 'PP';
				break;
				case('84'):
					$team = $awtm;
					$type = 'PP2';
				break;	
				default:
					die("Error with Scoring Summary. Could not retrieve Scoring Team Info.");
				break;
			}
			
			// Player that scored
			fseek ($fr,$tmpExtract + 4);
 			$GoalPlayer = (hexdec(bin2hex(fread($fr, 1))));
			$goalid = getPlayerID($team, $GoalPlayer, $stattype, $classic, 'S');
			
			// Assisters on Goal
			fseek ($fr,$tmpExtract + 5);
 			$GoalAst1 = (hexdec(bin2hex(fread($fr, 1))));
			if($GoalAst1 != 255)  // Assist occurred
				$a1id = getPlayerID($team, $GoalAst1, $stattype, $classic, 'S');
			else
				$a1id = 0;
			
			fseek ($fr,$tmpExtract + 6);
 			$GoalAst2 = (hexdec(bin2hex(fread($fr, 1))));
			if($GoalAst2 != 255)  // Assist occurred
				$a2id = getPlayerID($team, $GoalAst2, $stattype, $classic, 'S');
			else
				$a2id = 0;

			// Enter Scoring Summary into database
			
			$ssq = "INSERT INTO ScoreSum (Game_ID, League_ID, Sub_League, Team_ID, Period, Time, G, A1, A2, Type)
					VALUES ('$gameid', '$lg', '$sub', '$team', '$Period', '$time', '$goalid', '$a1id', '$a2id', '$type')";
			$ssr = @mysql_query($ssq) or die("Could not enter Score Summary.");
			
			$tmpExtract = ($tmpExtract + 6);  // move to next goal summary
		
		}
		
	}

/**********************************************************************************/
	
	// Penalty Summary
	
	if($stattype == "GENS"){
	
		$tmpExtract2 = 59989;		// Penalty Offset Start

		fseek ($fr,59989);
 	  	$EndofPS = hexdec(bin2hex(fread($fr, 1)));

		for ($i = 2; $i < (($EndofPS + 6) / 4); $i += 1){

			// Period of Penalty
			fseek ($fr,$tmpExtract2 + 1);
	 		$PenPer = (int) (hexdec(bin2hex(fread($fr, 1))) / 64) + 1;
 
			// Time of Penalty (in Seconds)
			fseek ($fr,$tmpExtract2 + 1);
		 	$PenSec =  (hexdec(bin2hex(fread($fr, 1))));
			fseek ($fr,$tmpExtract2 + 2);
 			$Pentmp =  (hexdec(bin2hex(fread($fr, 1))));
			$PenSec = $PenSec * 256 + $Pentmp - ($PenPer - 1) * 16384;
			$PenMin = (int) ($PenSec / 60);
			$PenSec = ($PenSec % 60);
			if($PenSec < 10)
				$Sec = '0'. $PenSec;
			else
				$Sec = $PenSec;
				
			$pentime = $PenMin. ":". $Sec;

			// Team that got Penalized
			fseek ($fr,$tmpExtract2 + 3);
	 		$PenTeam = (hexdec(bin2hex(fread($fr, 1)))); 
			
			if($PenTeam == 18 || $PenTeam == 22 || $PenTeam ==  24 || $PenTeam ==  26 || $PenTeam == 28 || $PenTeam == 30 
				|| $PenTeam == 32 || $PenTeam ==  34 || $PenTeam ==  36 || $PenTeam == 38) // Home
				$team = $hmtm;
			else 
				$team = $awtm;
		
			if($PenTeam == '18'  || $PenTeam == '146')
				$type = "Boarding";
			else if($PenTeam == '22' || $PenTeam == '150')
				$type = "Charging";
			else if($PenTeam == '24' || $PenTeam == '152')
				$type = "Slashing";
			else if($PenTeam == '26' || $PenTeam == '154') 			
				$type = "Roughing";
			else if($PenTeam == '28' || $PenTeam == '156') 
				$type = "Cross Check";
			else if($PenTeam == '30' || $PenTeam == '158')
				$type = "Hooking";
			else if($PenTeam == '32' || $PenTeam == '160')
				$type = "Tripping";
			else if($PenTeam == '34' || $PenTeam == '162')
				$type = "Interference";
			else if($PenTeam == '36' || $PenTeam == '164')
				$type = "Holding";
			else if($PenTeam == '38' || $PenTeam == '166')
				$type = "Holding";
				
			
			// Player
			
			fseek ($fr,$tmpExtract2 + 4);
	 		$PenPlayer = (hexdec(bin2hex(fread($fr, 1))));
			$penid = getPlayerID($team, $PenPlayer, $stattype, $classic, 'G');
						
			
			// Add to database
			
			$psq = "INSERT INTO PenSum (Game_ID, League_ID, Sub_League, Team_ID, Player_ID, Period, Time, Type)
					VALUES ('$gameid', '$lg', '$sub', '$team', '$penid', '$PenPer', '$pentime', '$type')";
			$psr = @mysql_query($psq) or die("Could not enter Penalty Summary.");
			
			if($blitz == 1){  // Blitz League
			
				$pq = "UPDATE PlayerStats SET PIM=PIM+2 WHERE Player_ID='$penid' AND Game_ID='$gameid' AND Team_ID='$team' LIMIT 1";
				$pr = @mysql_query($pq) or die("Could not update PIM.");
			}
			
			$tmpExtract2 = ($tmpExtract2 + 4);
		}
	
	}
	
	else if($stattype == "SNES"){
		
		$tmpExtract2 = 8784;		// Penalty Summary Start -1

		fseek ($fr,8783);
 	  	$EndofPS = hexdec(bin2hex(fread($fr, 1)));  // # of Penalties

		for ($i = 1; $i <= $EndofPS; $i += 1){

			// Period of Penalty
			fseek ($fr,$tmpExtract2 + 2);
	 		$PenPer = (int) (hexdec(bin2hex(fread($fr, 1))) / 64) + 1;
 
			// Time of Penalty (in Seconds)
			fseek ($fr,$tmpExtract2 + 2);
		 	$PenSec =  (hexdec(bin2hex(fread($fr, 1))));
			fseek ($fr,$tmpExtract2 + 1);
 			$Pentmp =  (hexdec(bin2hex(fread($fr, 1))));
			$PenSec = $PenSec * 256 + $Pentmp - ($PenPer - 1) * 16384;
			$PenMin = (int) ($PenSec / 60);
			$PenSec = ($PenSec % 60);
			if($PenSec < 10)
				$Sec = '0'. $PenSec;
			else
				$Sec = $PenSec;
				
			$pentime = $PenMin. ":". $Sec;

			// Team that got Penalized
			fseek ($fr,$tmpExtract2 + 4);
	 		$PenType = (hexdec(bin2hex(fread($fr, 1)))); 
			
			switch($PenType){
				
				case('20'):
					$type = "Roughing";
				break;
				case('24'):
					$type = "Charging";
				break;
				case('26'):
					$type = "Slashing";
				break;
				case('28'):
					$type = "Roughing";
				break;
				case('30'):
					$type = "Cross Check";
				break;
				case('32'):
					$type = "Hooking";
				break;
				case('34'):
					$type = "Tripping";
				break;
				case('38'):
					$type = "Interference";
				break;
				case('40'):
					$type = "Holding";
				break;	
				default:
					$type = "Penalty";
				break;
			}
			
			
			// Player
		
			fseek ($fr,$tmpExtract2 + 3);
	 		$PenPlayer = (hexdec(bin2hex(fread($fr, 1))));
			
//			echo $PenType. "*". $PenPlayer. " -- ";
			
			// Home or Away?
			
			if($PenPlayer <= 24){	// Home
				$PlayOff = $PenPlayer;
				$team = $hmtm;
			}
			else {
				$PlayOff = $PenPlayer - 128;
				$team = $awtm;
			}
//			echo $PlayOff. ":";	
			$penid = getPlayerID($team, $PlayOff, $stattype, $classic, 'S');
						
			
			// Add to database
			
			$psq = "INSERT INTO PenSum (Game_ID, League_ID, Sub_League, Team_ID, Player_ID, Period, Time, Type)
					VALUES ('$gameid', '$lg', '$sub', '$team', '$penid', '$PenPer', '$pentime', '$type')";
			//echo $psq. " ";
			$psr = @mysql_query($psq) or die("Could not enter Penalty Summary.");
			
			$tmpExtract2 = ($tmpExtract2 + 4);
		}
		
	}
	
/**********************************************************************************/
	
	// Plus/Minus for Blitz
	
	if($stattype == "GENS" && $blitz == 1){  // Calc Plus/Minus for Blitz
		
		$tmpExtract = 66413;	// Plus/Minus Info Length Offsets 66412, 66413
		fseek ($fr,66412);
  	 	$EndofPM = hexdec(bin2hex(fread($fr, 2)));
		
		for ($i=1;$i<(($EndofPM + 14) / 14);$i+=1){

			// Type of Goal, Team that Scored
			fseek ($fr,$tmpExtract + 1);		
			$GoalTeam = hexdec(bin2hex(fread($fr, 1)));
//			echo 'Goal Type: '. $GoalTeam. '<br />';
			switch($GoalTeam){
			
				case(0):
					$pm = 1;
					$type = 'SH2';
					$hplayers = 6;
					$aplayers = 6;
					$hpm = 1;
					$apm = -1;
				break;
				case(1):
					$pm = 1;
					$type = 'SH';
					$hplayers = 6;
					$aplayers = 6;
					$hpm = 1;
					$apm = -1;
				break;
				case(2):
					$pm = 1;
					$type = 'EV';
					$hplayers = 6;
					$aplayers = 6;
					$hpm = 1;
					$apm = -1;
				break;
				
				case(128):
					$pm = 1;
					$type = 'SH2';
					$hplayers = 6;
					$aplayers = 6;
					$hpm = -1;
					$apm = 1;
				break;
				case(129):
					$pm = 1;
					$type = 'SH';
					$hplayers = 6;
					$aplayers = 6;
					$hpm = -1;
					$apm = 1;
				break;
				case(130):
					$pm = 1;
					$type = 'EV';
					$hplayers = 6;
					$aplayers = 6;
					$hpm = -1;
					$apm = 1;
				break;
				
				default:
					$pm = 0;
				break;
			}
			
			if($pm == 1){  // Plus/Minus will be applied
				
				$hmonice = $tmpExtract + 3;
				$awonice = $tmpExtract + 9;
				
				// Retrieve Home Players and Add Plus/Minus
				
				for($j = 0;$j < $hplayers;$j++){
					
					fseek ($fr,$hmonice + $j);
 					$Player = (hexdec(bin2hex(fread($fr, 1))));
					
					if($Player != '0' && $Player != '1' && $Player != '255'){	// FF is in place of a player missing (like on a SH goal) or Goalie ( 0 or 1)
						$plid = getPlayerID($hmtm, $Player, $stattype, 0, 'G');
					
						$pmq = "UPDATE PlayerStats SET PlusMinus = PlusMinus + $hpm 
								WHERE Player_ID='$plid' AND Game_ID='$gameid' AND Team_ID='$hmtm' LIMIT 1";
						$pmr = @mysql_query($pmq) or die("Error:  Could not update Home Plus/Minus Stat.");
					}
				}
				
				// Away Players
				
				for($j = 0;$j < $aplayers;$j++){
					
					fseek ($fr,$awonice + $j);
 					$Player = (hexdec(bin2hex(fread($fr, 1))));
					
					if($Player != '0' && $Player != '1' && $Player != '255'){	// 255 is in place of a player missing (like on a SH goal) or Goalie ( 0 or 1)

						$plid = getPlayerID($awtm, $Player, $stattype, 0, 'G');
						$pmq = "UPDATE PlayerStats SET PlusMinus = PlusMinus + $apm
							WHERE Player_ID='$plid' AND Game_ID='$gameid' AND Team_ID='$awtm' LIMIT 1";
						$pmr = @mysql_query($pmq) or die("Error:  Could not update Away Plus/Minus Stat.");
					}
				}
			
		
			}
		
			$tmpExtract = ($tmpExtract + 14);  // move to next summary

		}
//	die();
	}	
	
}  // end of function

function updatepct($id, $type){		// Update PCT
	
	if($type == "1"){
		$tbl = "AllStats_Gens";
		$field = "User_ID";
	}	
	else if($type == "2"){
		$tbl = "AllStats_Snes";
		$field = "User_ID";
	}
	else {
		$tbl = "Standings";
		$field = "Team_ID";
	}
	
	$pctq = "SELECT GP, W, T FROM $tbl WHERE $field = '$id' LIMIT 1";		
	$pctr = @mysql_query($pctq);
	
	if($pctr){
		$pctrow = mysql_fetch_array($pctr, MYSQL_ASSOC);
		$pct = ($pctrow['W'] / ($pctrow['GP'] - $pctrow['T']));	
	
		$uppctq = "UPDATE $tbl SET PCT = '$pct' WHERE $field = '$id' LIMIT 1";
		$uppctr = @mysql_query($uppctq);

	}

}  // end of function	

function updategaa($id, $type){		// Update GAA, GFA

	if($type == "1"){
		$tbl = "AllStats_Gens";
		$field = "User_ID";
	}	
	else if($type == "2"){
		$tbl = "AllStats_Snes";
		$field = "User_ID";
	}
	else {
		$tbl = "Standings";
		$field = "Team_ID";
	}
	
	$gaaq = "SELECT GF, GA, GP, OTW, OTL, T FROM $tbl WHERE $field = '$id' LIMIT 1";
	$gaar = mysql_query($gaaq) or die("ERROR 2012: Could not retreive GAA/GFA info.");
	
	
	$gaarow = mysql_fetch_array($gaar, MYSQL_ASSOC);
	$denom = $gaarow['GP'] + (($gaarow['OTW'] + $gaarow['OTL'] + $gaarow['T']) / 3.5);	
	$gaa = ($gaarow['GA'] / $denom);
	$gfa = ($gaarow['GF'] / $denom);
		
	$upgq = "UPDATE $tbl SET GAA = '$gaa', GFA = '$gfa' WHERE $field = '$id' LIMIT 1";
	$upgr = mysql_query($upgq) or die("ERROR 2013: Could not update GAA/GFA.");	
	
	
}  // end of function

function updatelast10($tmid){

	$last10q = "SELECT S.H_Score Hmsc, S.A_Score Awsc, S.Home, S.Away, S.OT
			FROM Schedule S
			WHERE (S.Home = '$tmid' OR S.Away = '$tmid') AND S.H_Confirm = '1' AND S.A_Confirm = '1' 
			ORDER BY S.Confirm_Time DESC LIMIT 10";

	$last10r = mysql_query($last10q) or die("ERROR 2016: Could not retrieve last 10 games.");
	$win = $loss = $tie = 0; 		
	
	while($row = mysql_fetch_array($last10r, MYSQL_ASSOC)){
		if(($row['Hmsc'] == $row['Awsc']) && $row['OT'] == 1)  // tie	
			$tie += 1;
		else if($row['Hmsc'] > $row['Awsc'] && $row['Home'] == $tmid)  // team home win
			$win += 1;
		else if($row['Hmsc'] > $row['Awsc'] && $row['Away'] == $tmid)  // team away loss
			$loss += 1;
		else if($row['Awsc'] > $row['Hmsc'] && $row['Home'] == $tmid)  // team home loss
			$loss += 1;
		else if($row['Awsc'] > $row['Hmsc'] && $row['Away'] == $tmid) // team away win
			$win += 1;
	}

	$last10 = $win. "-". $loss. "-". $tie;
	
	return $last10;
		
}  // end of function
	 
function updaterec($hm, $aw, $hmsc, $awsc, $all){
	
	$hmrecq = "SELECT S.HR, S.CR, S.DR, T.User_ID, T.Conf_Div_ID, C.Conference Conf, A.HR AHR 
			FROM Standings S
			JOIN Teams T ON S.Team_ID = T.Team_ID
			JOIN Conf_Div C ON T.Conf_Div_ID = C.Conf_Div_ID
			JOIN $all A ON T.User_ID = A.User_ID
			WHERE S.Team_ID = '$hm' LIMIT 1";

	$awrecq = "SELECT S.AR, S.CR, S.DR, T.User_ID, T.Conf_Div_ID, C.Conference Conf, A.AR AAR
			FROM Standings S
			JOIN Teams T ON S.Team_ID = T.Team_ID
			JOIN Conf_Div C ON T.Conf_Div_ID = C.Conf_Div_ID
			JOIN $all A ON T.User_ID = A.User_ID
			WHERE S.Team_ID = '$aw' LIMIT 1";

	$hmrecr = mysql_query($hmrecq) or die("ERROR 2014: Could not retrieve records.");
	$awrecr = mysql_query($awrecq) or die("ERROR 2015: Could not retrieve records.");
	
	$hmrow = mysql_fetch_array($hmrecr, MYSQL_ASSOC);
	$awrow = mysql_fetch_array($awrecr, MYSQL_ASSOC);

	$hhr = explode("-", $hmrow['HR']);
	$hcr = explode("-", $hmrow['CR']);
	$hdr = explode("-", $hmrow['DR']);
	$ahhr = explode("-", $hmrow['AHR']);	
	$awr = explode("-", $awrow['AR']);
	$acr = explode("-", $awrow['CR']);
	$adr = explode("-", $awrow['DR']);
	$aawr = explode("-", $awrow['AAR']);
	
	if($hmsc == $awsc){  // Tie
		$hhr[2] += 1;
		$awr[2] += 1;
		$ahhr[2] += 1;
		$aawr[2] += 1;
		
		if($hmrow['Conf_Div_ID'] == $awrow['Conf_Div_ID']){  // Division game
			$hdr[2] += 1;
			$adr[2] += 1;
		}
		if($hmrow['Conf'] == $awrow['Conf']){  // Conference game
			$hcr[2] += 1;
			$acr[2] += 1;
		}
	}
	else if($hmsc > $awsc){  // Home win
		$hhr[0] += 1;
		$awr[1] += 1;
		$ahhr[0] += 1;
		$aawr[1] += 1;

		if($hmrow['Conf_Div_ID'] == $awrow['Conf_Div_ID']){  // Division game
			$hdr[0] += 1;
			$adr[1] += 1;
		}
		if($hmrow['Conf'] == $awrow['Conf']){  // Conference game
			$hcr[0] += 1;
			$acr[1] += 1;
		}
	}	
	else if($awsc > $hmsc){  // Away win
		$hhr[1] += 1;
		$awr[0] += 1;
		$ahhr[1] += 1;
		$aawr[0] += 1;

		if($hmrow['Conf_Div_ID'] == $awrow['Conf_Div_ID']){  // Division game
			$hdr[1] += 1;
			$adr[0] += 1;
		}
		if($hmrow['Conf'] == $awrow['Conf']){  // Conference game
			$hcr[1] += 1;
			$acr[0] += 1;
		}
	}

	$hm10 = updatelast10($hm);
	$aw10 = updatelast10($aw);

	$hhrec = implode("-", $hhr);
	$hcrec = implode("-", $hcr);
	$hdrec = implode("-", $hdr);
	$ahhrec = implode("-", $ahhr);	
	$awrec = implode("-", $awr);
	$acrec = implode("-", $acr);
	$adrec = implode("-", $adr);
	$aawrec = implode("-", $aawr);		

	$hmrupq = "UPDATE Standings SET HR= '$hhrec', CR= '$hcrec', DR= '$hdrec', Last10= '$hm10' 
			WHERE Team_ID= '$hm' LIMIT 1";
	
	$awrupq = "UPDATE Standings SET AR= '$awrec', CR= '$acrec', DR= '$adrec', Last10= '$aw10' 
			WHERE Team_ID= '$aw' LIMIT 1";

	$ahmrupq = "UPDATE $all SET HR= '$ahhrec' WHERE User_ID='{$hmrow['User_ID']}' LIMIT 1";
	
	$aawrupq = "UPDATE $all SET AR= '$aawrec' WHERE User_ID='{$awrow['User_ID']}' LIMIT 1";

	$hmrupr = mysql_query($hmrupq) or die("ERROR 2017: Could not update records.");
	$awrupr = mysql_query($awrupq) or die("ERROR 2018: Could not update records.");
	$ahmrupr = mysql_query($ahmrupq) or die("ERROR 2019: Could not update records.");
	$aawrupr = mysql_query($aawrupq) or die("ERROR 2020: Could not update records.");

	if(!($hmrupr && $awrupr && $ahmrupr && $aawrupr))
		die("ERROR 2021:  Update record error.");

	mysql_free_result($hmrecr);
	mysql_free_result($awrecr);
	
}  // end of function



function updatestats($gameid, $tm, $team){
	
	$blitz = blitzChk($lg);
	
	// Retrieve Game Stats
	
	$gsq = "SELECT *, TIME_TO_SEC(AZH) AS AZHT, TIME_TO_SEC(AZA) AS AZAT FROM GameStats WHERE Game_ID='$gameid' LIMIT 1";
	$gsr = @mysql_query($gsq) or die("Could not retrieve Game Stats.  Please contact the administrator.");
	
	// Retrieve info to calculate Attack Zone Times
	
	$azq = "SELECT GP, TIME_TO_SEC(AZ) AS AZTS, TIME_TO_SEC(AZA) AZATS FROM Standings WHERE Team_ID='$tm'";
	$azr = @mysql_query($azq) or die("Could not retrieve Attack Zone Stats.  Please contact the administrator.");
	
	$row = mysql_fetch_array($gsr, MYSQL_ASSOC);
	$arow = mysql_fetch_array($azr, MYSQL_ASSOC);
	
	if(substr($row['Sub_League'], 0, 4) == "GENS")
		$stattype = "GENS";
	else if (substr($row['Sub_League'], 0, 4) == "SNES")
		$stattype = "SNES";
	else
		die("Error: Problem with game data.  Please contact administrator.");
	
	if($team == 'H'){  // Team is home
		$shotsfor = $row['SHP1'] + $row['SHP2'] + $row['SHP3'] + $row['SHOT'];
		$shotsag = $row['SAP1'] + $row['SAP2'] + $row['SAP3'] + $row['SAOT'];
		$goalsag = $row['GAP1'] + $row['GAP2'] + $row['GAP3'] + $row['GAOT'];
		
		// Calculate Attack Zone Averages
		
		$gp = $arow['GP'] - 1;  // Game had already been submitted, 1 needs to be removed
		if($gp > '0'){  // Games have been played
			//$atkfavg = ((strtotime($arow['AZ']) * $gp) + strtotime($row['AZH'])) / $arow['GP'];
			//$atkaavg = ((strtotime($arow['AZA']) * $gp) + strtotime($row['AZA'])) / $arow['GP'];
			// $timef = date('H:i:s',$atkfavg);
			// $timea = date('H:i:s',$atkaavg);
			$atkfavg = (($arow['AZTS'] * $gp) + $row['AZHT']) / $arow['GP'];
			$atkaavg = (($arow['AZATS'] * $gp) + $row['AZAT']) / $arow['GP'];
			$timef = $atkfavg;
			$timea = $atkaavg;
		
		}
		else {
			
			$timef = $row['AZHT'];
			$timea = $row['AZAT'];
		}
		
		// Update strings
		
		$goalf = ", GP1=GP1+'{$row['GHP1']}', GP2=GP2+'{$row['GHP2']}', GP3=GP3+'{$row['GHP3']}', GOT=GOT+'{$row['GHOT']}'";
		$goalaf = ", GA1=GA1+'{$row['GAP1']}', GA2=GA2+'{$row['GAP2']}', GA3=GA3+'{$row['GAP3']}', GAOT=GAOT+'{$row['GAOT']}'";
		$shotf = ", SP1=SP1+'{$row['SHP1']}', SP2=SP2+'{$row['SHP2']}', SP3=SP3+'{$row['SHP3']}', SOT=SOT+'{$row['SHOT']}'";
		$shotaf = ", SA1=SA1+'{$row['SAP1']}', SA2=SA2+'{$row['SAP2']}', SA3=SA3+'{$row['SAP3']}', SAOT=SAOT+'{$row['SAOT']}'";
		$pf = ", PIM=PIM+'{$row['PIMH']}', PIMA=PIMA+'{$row['PIMA']}', PPG=PPG+'{$row['PPHG']}', PPGA=PPGA+'{$row['PPAG']}' ,
				PP=PP+'{$row['PPH']}', PK=PK+'{$row['PPA']}', SHG=SHG+'{$row['SHHG']}', SHGA=SHGA+'{$row['SHAG']}'";
		$bcf = ", BCF=BCF+'{$row['BCH']}', BCA=BCA+'{$row['BCA']}'";
		$specf = ", BAG=BAG+'{$row['BAHG']}', BA=BA+'{$row['BAH']}', 1TG=1TG+'{$row['1THG']}', 1TA=1TA+'{$row['1TH']}',
					PSG=PSG+'{$row['PSHG']}', PS=PS+'{$row['PSH']}', FOW=FOW+'{$row['FOH']}', FOA=FOA+'{$row['FOA']}',
					PSC=PSC+'{$row['PCH']}', PSA=PSA+'{$row['PH']}'";
		$atf = ", AZ=SEC_TO_TIME('$timef'), AZA=SEC_TO_TIME('$timea')";	
	
	}
	
	else {  // Team is away
	
		$shotsfor = $row['SAP1'] + $row['SAP2'] + $row['SAP3'] + $row['SAOT'];
		$shotsag = $row['SHP1'] + $row['SHP2'] + $row['SHP3'] + $row['SHOT'];
		$goalsag = $row['GHP1'] + $row['GHP2'] + $row['GHP3'] + $row['GHOT'];

		
		// Calculate Attack Zone Averages
		
	$gp = $arow['GP'] - 1;  // Game had already been submitted, 1 needs to be removed
		if($gp > '0'){  // Games have been played

			//$atkfavg = ((strtotime($arow['AZ']) * $gp) + strtotime($row['AZA'])) / $arow['GP'];
			//$atkaavg = ((strtotime($arow['AZA']) * $gp) + strtotime($row['AZH'])) / $arow['GP'];
			//$timef = date('H:i:s',$atkfavg);
			//$timea = date('H:i:s',$atkaavg);
			$atkfavg = (($arow['AZTS'] * $gp) + $row['AZAT']) / $arow['GP'];
			$atkaavg = (($arow['AZATS'] * $gp) + $row['AZHT']) / $arow['GP'];
			$timef = $atkfavg;
			$timea = $atkaavg;
		
		}
		else {
			
			$timef = $row['AZAT'];
			$timea = $row['AZHT'];
		}
				

		// Update strings
		
		$goalf = ", GP1=GP1+'{$row['GAP1']}', GP2=GP2+'{$row['GAP2']}', GP3=GP3+'{$row['GAP3']}', GOT=GOT+'{$row['GAOT']}'";
		$goalaf = ", GA1=GA1+'{$row['GHP1']}', GA2=GA2+'{$row['GHP2']}', GA3=GA3+'{$row['GHP3']}', GAOT=GAOT+'{$row['GHOT']}'";
		$shotf = ", SP1=SP1+'{$row['SAP1']}', SP2=SP2+'{$row['SAP2']}', SP3=SP3+'{$row['SAP3']}', SOT=SOT+'{$row['SAOT']}'";
		$shotaf = ", SA1=SA1+'{$row['SHP1']}', SA2=SA2+'{$row['SHP2']}', SA3=SA3+'{$row['SHP3']}', SAOT=SAOT+'{$row['SHOT']}'";
		$pf = ", PIM=PIM+'{$row['PIMA']}', PIMA=PIMA+'{$row['PIMH']}', PPG=PPG+'{$row['PPAG']}', PPGA=PPGA+'{$row['PPHG']}',
				PP=PP+'{$row['PPA']}', PK=PK+'{$row['PPH']}', SHG=SHG+'{$row['SHAG']}', SHGA=SHGA+'{$row['SHHG']}'";
		$bcf = ", BCF=BCF+'{$row['BCA']}', BCA=BCA+'{$row['BCH']}'";
		$specf = ", BAG=BAG+'{$row['BAAG']}', BA=BA+'{$row['BAA']}', 1TG=1TG+'{$row['1TAG']}', 1TA=1TA+'{$row['1TA']}',
					PSG=PSG+'{$row['PSAG']}', PS=PS+'{$row['PSA']}', FOW=FOW+'{$row['FOA']}', FOA=FOA+'{$row['FOH']}',
					PSC=PSC+'{$row['PCA']}', PSA=PSA+'{$row['PA']}'";
		$atf = ", AZ=SEC_TO_TIME('$timef'), AZA=SEC_TO_TIME('$timea')";
	
	}
	
//	echo '>'. $atf. ' ';

	$upq = "UPDATE Standings SET SF=SF+'$shotsfor', SA=SA+'$shotsag' $goalf $goalaf $shotf $shotaf $pf $bcf $specf $atf 
			WHERE Team_ID='$tm' LIMIT 1";
	$upr = @mysql_query($upq) or die ("Could not update Standings with Game Stats.  Please contact administrator.");
	//echo $upq;
	if(!$upr)
		die("Standings with Game Stats not updated.  Please contact administrator.");
		
	// Update Season Player Stats
	
	$psq = "SELECT * FROM PlayerStats WHERE Game_ID='$gameid' AND Team_ID='$tm'";
	$psr = @mysql_query($psq) or die("Could not select Player Stats.");
	
	while($row = mysql_fetch_array($psr, MYSQL_ASSOC)){
		
		$pid = $row['Player_ID'];
		$pg = 0;
		$pa = 0;
		$sg = 0;
		$sa = 0;
		
		// Goals/Assists Scored?
		if($row['G'] > 0 || $row['A'] > 0){
			// Find type of goals/assists (PP or SH?)
			
				$pr= @mysql_query("SELECT COUNT(*) PP FROM ScoreSum WHERE Game_ID='$gameid' AND G='$pid' AND Type LIKE 'PP%'");
				$prow = mysql_fetch_array($pr, MYSQL_ASSOC);
				$pg = $prow['PP'];
				$pr= @mysql_query("SELECT COUNT(*) PPA FROM ScoreSum WHERE Game_ID='$gameid' AND A1='$pid' AND Type LIKE 'PP%'");
				$prow = mysql_fetch_array($pr, MYSQL_ASSOC);
				$pa = $prow['PPA'];
				$pr= @mysql_query("SELECT COUNT(*) PPA FROM ScoreSum WHERE Game_ID='$gameid' AND A2='$pid' AND Type LIKE 'PP%'");
				$prow = mysql_fetch_array($pr, MYSQL_ASSOC);
				$pa += $prow['PPA'];
				$pr= @mysql_query("SELECT COUNT(*) SH FROM ScoreSum WHERE Game_ID='$gameid' AND G='$pid' AND Type LIKE 'SH%'");
				$prow = mysql_fetch_array($pr, MYSQL_ASSOC);
				$sg = $prow['SH'];
				$pr= @mysql_query("SELECT COUNT(*) SHA FROM ScoreSum WHERE Game_ID='$gameid' AND A1='$pid' AND Type LIKE 'SH%'");
				$prow = mysql_fetch_array($pr, MYSQL_ASSOC);
				$sa = $prow['SHA'];
				$pr= @mysql_query("SELECT COUNT(*) SHA FROM ScoreSum WHERE Game_ID='$gameid' AND A2='$pid' AND Type LIKE 'SH%'");
				$prow = mysql_fetch_array($pr, MYSQL_ASSOC);
				$sa = $prow['SHA'];						
		}
		
		
		// TOI Averaging
		if($stattype == 'GENS'){
			
			$toiq = "SELECT COUNT(*) GP, SEC_TO_TIME(AVG(TIME_TO_SEC(TOI))) AS TOIAVG FROM PlayerStats WHERE Team_ID='$tm' AND Player_ID='$pid'";
			$toir = @mysql_query($toiq);
			$trow = mysql_fetch_array($toir, MYSQL_ASSOC);
	
/*			$gp = $trow['GP'];  // Game had already been submitted, 1 needs to be removed
			if($gp > '0' && $trow['TOI'] != '00:00:00'){  // Games have been played
			
	//			echo $pid. ":". (strtotime($trow['TOI'])) . " * ". $gp. " + ". (strtotime($row['TOI'])). " / ". $trow['GP']. '+1 >';                                  
				$toiavg = ((strtotime($trow['TOI']) * $gp) + strtotime($row['TOI'])) / ($trow['GP'] + 1);  // add new game	
	//			echo $toiavg. " = ";
				$toi = date('H:i:s',$toiavg);
			}
			else 
				$toi = $row['TOI'];
		}
	//		echo $toi. " ---";
*/
		$toi = $trow['TOIAVG'];
		
		}
		else
			$toi = "00:00:00";

//		echo '>'. $toi. '-'. $trow['GP']; 
		// Update database
		
		$p = $row['G'] + $row['A'];
		$pp = $pg + $pa;
		
		// Shutout?
		
		if($goalsag == 0)
			$so = 1;
		else
			$so = 0;
		
		
		$upq = "UPDATE SeasonStats SET GP=GP+1, G=G+'{$row['G']}', A=A+'{$row['A']}', P=P+'$p', SOG=SOG+'{$row['SOG']}', 
				PIM=PIM+'{$row['PIM']}', Chks=Chks+'{$row['Chks']}', PPG=PPG+'$pg', PPA=PPA+'$pa', PPT=PPT+'$pp', 
				SHG=SHG+'$sg', SHA=SHA+'$sa', SO=SO+'$so', TOI='$toi' WHERE Team_ID='$tm' AND Player_ID='$pid' LIMIT 1";
		$upr = @mysql_query($upq) or die("Could not update SeasonStats.");
	}
	
	
}  // end of function

function updatestd($gameid, $lg){	// Updates standings

	$ptsq = "SELECT * FROM Points WHERE League_ID = '$lg' LIMIT 1";	// Points values

	$ptsr = @mysql_query($ptsq) or die('ERROR 2007: Could not retrieve points.');
	
	$gmq = "SELECT S.Home Hm, S.Away Aw, S.H_Score HmS, S.A_Score AwS, S.OT, H.Streak HStrk, A.Streak AStrk
		FROM Schedule S
		JOIN Standings H ON H.Team_ID = S.Home
		JOIN Standings A ON A.Team_ID = S.Away 
		WHERE Game_ID = '$gameid' LIMIT 1";	// Game info. 
	

	$gmr = mysql_query($gmq) or die('ERROR 2008: Game has not been submitted.');
		
	if($gmr && $ptsr){  // Interpret outcome of game
		$gm = mysql_fetch_array($gmr, MYSQL_ASSOC);
		$pts = mysql_fetch_array($ptsr, MYSQL_ASSOC);
				
		if($gm['HmS'] == $gm['AwS'] && $gm['OT'] == 1){	// Tie
			$upqh = "T=T+1, GF=GF+'{$gm['HmS']}', GA=GA+'{$gm['AwS']}', 
				PTS=PTS+'{$pts['T']}'+'{$pts['GP']}'";
			$upquh = "T=T+1, GF=GF+'{$gm['HmS']}', GA=GA+'{$gm['AwS']}'";
			$upqa = "T=T+1, GF=GF+'{$gm['HmS']}', GA=GA+'{$gm['AwS']}', 
				PTS=PTS+'{$pts['T']}'+'{$pts['GP']}'";
			$upqua = "T=T+1, GF=GF+'{$gm['HmS']}', GA=GA+'{$gm['AwS']}'";		
		}	
		else if($gm['HmS'] > $gm['AwS'] && $gm['OT'] == 1){  // Home ot win
			if($gm['HStrk'] > 0)	// Streak update
				$hstrk = "Streak = Streak+1";
			else 
				$hstrk = "Streak = 1";
			if($gm['AStrk'] < 0)
				$astrk = "Streak = Streak-1";
			else
				$astrk = "Streak = -1";
			
			$upqh = "W=W+1, OTW=OTW+1, GF=GF+'{$gm['HmS']}', GA=GA+'{$gm['AwS']}', 
				PTS=PTS+'{$pts['OTW']}'+'{$pts['GP']}', $hstrk";
			$upquh = "W=W+1, OTW=OTW+1, GF=GF+'{$gm['HmS']}', GA=GA+'{$gm['AwS']}'";
			$upqa = "L=L+1, OTL=OTL+1, GF=GF+'{$gm['AwS']}', GA=GA+'{$gm['HmS']}', 
				PTS=PTS+'{$pts['OTL']}'+'{$pts['GP']}', $astrk";
			$upqua = "L=L+1, OTL=OTL+1, GF=GF+'{$gm['AwS']}', GA=GA+'{$gm['HmS']}'";		
		}	

		else if($gm['AwS'] > $gm['HmS'] && $gm['OT'] == 1){  // away ot win
			if($gm['AStrk'] > 0)	// Streak update
				$astrk = "Streak = Streak+1";
			else 
				$astrk = "Streak = 1";
			if($gm['HStrk'] < 0)
				$hstrk = "Streak = Streak-1";
			else
				$hstrk = "Streak = -1";
			
			$upqh = "L=L+1, OTL=OTL+1, GF=GF+'{$gm['HmS']}', GA=GA+'{$gm['AwS']}', 
				PTS=PTS+'{$pts['OTL']}'+'{$pts['GP']}', $hstrk";
			$upquh = "L=L+1, OTL=OTL+1, GF=GF+'{$gm['HmS']}', GA=GA+'{$gm['AwS']}'";			
			$upqa = "W=W+1, OTW=OTW+1, GF=GF+'{$gm['AwS']}', GA=GA+'{$gm['HmS']}', 
				PTS=PTS+'{$pts['OTW']}'+'{$pts['GP']}', $astrk";
			$upqua = "W=W+1, OTW=OTW+1, GF=GF+'{$gm['AwS']}', GA=GA+'{$gm['HmS']}'";
		}	
		else if($gm['HmS'] > $gm['AwS'] && $gm['OT'] == 0){  // home win
			if($gm['HStrk'] > 0)	// Streak update
				$hstrk = "Streak = Streak+1";
			else 
				$hstrk = "Streak = 1";
			if($gm['AStrk'] < 0)
				$astrk = "Streak = Streak-1";
			else
				$astrk = "Streak = -1";
			
			$upqh = "W=W+1, GF=GF+'{$gm['HmS']}', GA=GA+'{$gm['AwS']}', 
				PTS=PTS+'{$pts['W']}'+'{$pts['GP']}', $hstrk";
			$upquh = "W=W+1, GF=GF+'{$gm['HmS']}', GA=GA+'{$gm['AwS']}'";			
			$upqa = "L=L+1, GF=GF+'{$gm['AwS']}', GA=GA+'{$gm['HmS']}', 
				PTS=PTS+'{$pts['L']}'+'{$pts['GP']}', $astrk";
			$upqua = "L=L+1, GF=GF+'{$gm['AwS']}', GA=GA+'{$gm['HmS']}'";
		}	
		else if($gm['AwS'] > $gm['HmS'] && $gm['OT'] == 0){  // away win
			if($gm['AStrk'] > 0)	// Streak update
				$astrk = "Streak = Streak+1";
			else 
				$astrk = "Streak = 1";
			if($gm['HStrk'] < 0)
				$hstrk = "Streak = Streak-1";
			else
				$hstrk = "Streak = -1";
			
			$upqh = "L=L+1, GF=GF+'{$gm['HmS']}', GA=GA+'{$gm['AwS']}', 
				PTS=PTS+'{$pts['L']}'+'{$pts['GP']}', $hstrk";
			$upquh = "L=L+1, GF=GF+'{$gm['HmS']}', GA=GA+'{$gm['AwS']}'";
			$upqa = "W=W+1, GF=GF+'{$gm['AwS']}', GA=GA+'{$gm['HmS']}', 
				PTS=PTS+'{$pts['W']}'+'{$pts['GP']}', $astrk";
			$upqua = "W=W+1, GF=GF+'{$gm['AwS']}', GA=GA+'{$gm['HmS']}'";
		}
		else {
			return 9;
		}	
		
		// Update league database

		// Home Team		
		$upinfo = $upqh;
		$tm = $gm['Hm'];
		$upq = "UPDATE Standings SET GP=GP+1, $upinfo WHERE Team_ID = '$tm' LIMIT 1";		
		$upr = mysql_query($upq) or die("ERROR 2008: Home Team update error.");
		updatepct($tm, 0);
		updategaa($tm, 0);
		updatestats($gameid, $tm, 'H');			
		
		
		// Away Team		
		$upinfo = $upqa;
		$tm = $gm['Aw'];
		$upq = "UPDATE Standings SET GP=GP+1, $upinfo WHERE Team_ID = '$tm' LIMIT 1";		
		$upr = mysql_query($upq) or die("ERROR 2009: Away Team update error.");
		updatepct($tm, 0);
		updategaa($tm, 0);
		updatestats($gameid, $tm, 'A');
		
				
		// Update User All-Time Record

		$usrq = "SELECT H.User_ID HU, A.User_ID AU, S.Home Hm, S.Away Aw, S.Sub_League FROM Schedule S
			JOIN Teams H ON H.Team_ID = S.Home
			JOIN Teams A ON A.Team_ID = S.Away
			WHERE S.Game_ID = '$gameid' LIMIT 1";
		$usrr = mysql_query($usrq) or die("ERROR 2010: ATR retrieval error.");
		$uinfo = mysql_fetch_array($usrr, MYSQL_ASSOC);
						
		$sys = chkSys($uinfo['Sub_League']);
		
		if($sys == 'GENS'){
			$uptype = 1;
			$all = 'AllStats_Gens';
		}
		else {
			$uptype = 2;
			$all = 'AllStats_Snes';
		}
		// Home Team
		$upinfo = $upquh;
		$user = $uinfo['HU'];
		$upq = "UPDATE $all SET GP=GP+1, $upinfo WHERE User_ID = '$user' LIMIT 1";		
		
		$upr = mysql_query($upq) or die("ERROR 2011: Home ATR update error.");
		
		updatepct($user, $uptype);
		updategaa($user, $uptype);
		
		
		// Away Team
		$upinfo = $upqua;
		$user = $uinfo['AU'];
		$upq = "UPDATE $all SET GP=GP+1, $upinfo WHERE User_ID = '$user' LIMIT 1";		
		
		$upr = @mysql_query($upq);
		
		updatepct($user, $uptype);
		updategaa($user, $uptype);

		updaterec($gm['Hm'], $gm['Aw'], $gm['HmS'], $gm['AwS'], $all);		
		
		 		
	}
	else
		die("Game information not found.  Please contact the administrator.");
	
		
	mysql_free_result($ptsr);
	mysql_free_result($gmr);
	mysql_free_result($usrr);

	return 0;

}  // end of function

function enterstats($lg, $gameid, $teamid){	
		
		

}  // end of function

/*********************************************************************/

// retrieve POST variables


		
	$gameid = $_POST['gameid'];
	$teamid = $_POST['teamid'];
	$pwd = $_POST['pwd'];
	$gn = $_POST['gn'];

	$chk = errorcheck($teamid, $gameid, $pwd, $lg);
	
	if(!$chk)
		$chk = addgame($teamid, $gameid, $lg);
	else
		$error = $chk;
	
	if(!$chk)
		$chk = updatestd($gameid, $lg);
	else
		$error = $chk;	
	
	if(!$chk)  // pass OK
		$error = 0;
	else 
		$error = $chk;	

	$host = $_SERVER['HTTP_HOST'];
	header("Location: http://".$host."/html/log_a_state.php?lg=". $lg. "&gmid=". $gameid. "&tmid=". $teamid. 
		"&gn=". $gn. "&err=". $error);


?>
