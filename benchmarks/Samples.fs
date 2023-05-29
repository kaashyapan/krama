namespace JsonBenchmarks

module Sample =
    let sampleString =
        """
        {
            	"_type": "News",
            	"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/news\/search?q=Elephants",
            	"queryContext": {
            		"originalQuery": "Elephants",
            		"adultIntent": false
            	},
            	"totalEstimatedMatches": 20,
            	"sort": [
            		{
            			"name": "Best match",
            			"id": "relevance",
            			"isSelected": false,
            			"url": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/news\/search?q=Elephants"
            		},
            		{
            			"name": "Most recent",
            			"id": "date",
            			"isSelected": true,
            			"url": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/news\/search?q=Elephants&sortby=date"
            		}
            	],
            	"value": [
            		{
            			"name": "Wild Elephants Surprise Campers And Drink Water From Pool",
            			"url": "https:\/\/www.sharedots.com\/wild-elephants-surprise-campers-drink-water-from-pool-29738.html",
            			"image": {
            				"thumbnail": {
            					"contentUrl": "https:\/\/www.bing.com\/th?id=OVFT.L38BimXDWY8TYRKWdKailS&pid=News",
            					"width": 560,
            					"height": 292
            				}
            			},
            			"description": "An amazing moment is captured on camera when a herd of wild elephants drink from a pool in the Somalisa Tent Camp in Hwange NP, Zimbabwe.",
            			"about": [
            				{
            					"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/entities\/740cd184-dd27-820a-1a75-62300463698c",
            					"name": "Surprise"
            				},
            				{
            					"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/entities\/85883125-fe46-e810-e1b9-f83d7d6ffe1b",
            					"name": "Elephant"
            				}
            			],
            			"provider": [
            				{
            					"_type": "Organization",
            					"name": "Sharedots"
            				}
            			],
            			"datePublished": "2021-04-06T18:31:00.0000000Z",
            			"video": {
            				"name": "Wild Elephants Surprise Campers And Drink Water From Pool",
            				"thumbnailUrl": "https:\/\/www.bing.com\/th?id=OVF.ZsVDAsAGBjJt74FxEwyyRA&pid=News",
            				"thumbnail": {
            					"width": 300,
            					"height": 156
            				}
            			}
            		},
            		{
            			"name": "New elephant habitat ready for West Midland Safari Park reopening",
            			"url": "https:\/\/www.kidderminstershuttle.co.uk\/news\/19212763.new-elephant-habitat-ready-west-midland-safari-park-reopening\/",
            			"description": "The park’s African elephants will now be visible for safari drive-through guests at their new habitat, which has been purpose-built into the long-awaited Safari Lodges development. The area includes a vast drinking pool and waterfall which will be at the ...",
            			"about": [
            				{
            					"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/entities\/47f4ae05-29d0-93df-ca63-25b1d095b3c5",
            					"name": "West Midland Safari Park"
            				}
            			],
            			"provider": [
            				{
            					"_type": "Organization",
            					"name": "The Shuttle"
            				}
            			],
            			"datePublished": "2021-04-06T17:25:00.0000000Z"
            		},
            		{
            			"name": "Botswana trophy hunting season opens after COVID hiatus",
            			"url": "https:\/\/ewn.co.za\/2021\/04\/06\/botswana-trophy-hunting-season-opens-after-covid-hiatus",
            			"image": {
            				"thumbnail": {
            					"contentUrl": "https:\/\/www.bing.com\/th?id=OVFT.zQbeTm45lns7nETjU7vKai&pid=News",
            					"width": 700,
            					"height": 437
            				}
            			},
            			"description": "Licences have been issued to kill 287 elephants, the biggest category of animals, according to the authorities. The landlocked southern African country boasts the world's largest elephant ...",
            			"about": [
            				{
            					"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/entities\/8aa19277-27e8-6def-a3e1-47929d921d30",
            					"name": "Botswana"
            				},
            				{
            					"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/entities\/cd9a1839-8ef6-6cd5-592c-6dc77a8c7ac9",
            					"name": "Trophy hunting"
            				},
            				{
            					"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/entities\/7ca0745d-16b0-6763-d9ee-a85296a9788d",
            					"name": "Hunting season"
            				}
            			],
            			"provider": [
            				{
            					"_type": "Organization",
            					"name": "EWN Sport"
            				}
            			],
            			"datePublished": "2021-04-06T16:49:00.0000000Z",
            			"category": "ScienceAndTechnology"
            		},
            		{
            			"name": "Botswana allows hunting of 287 elephants",
            			"url": "https:\/\/thewest.com.au\/news\/animals\/botswana-allows-hunting-of-287-elephants-c-2529706",
            			"image": {
            				"thumbnail": {
            					"contentUrl": "https:\/\/www.bing.com\/th?id=OVFT.NGdes47Ix2UhNzGYx3EQ7C&pid=News",
            					"width": 700,
            					"height": 393
            				}
            			},
            			"description": "Hunters in Botswana will be allowed to shoot 287 elephants as the country opens the hunting season, angering animal activists.",
            			"provider": [
            				{
            					"_type": "Organization",
            					"name": "The West Australian",
            					"image": {
            						"thumbnail": {
            							"contentUrl": "https:\/\/www.bing.com\/th?id=AR_40233afbc35f05630a6f47f29b80a773&pid=news"
            						}
            					}
            				}
            			],
            			"datePublished": "2021-04-06T16:03:00.0000000Z",
            			"category": "World"
            		},
            		{
            			"name": "Why Madhya Pradesh is witnessing uncontrolled forest fires | India Today Insight",
            			"url": "https:\/\/www.indiatoday.in\/india-today-insight\/story\/why-madhya-pradesh-is-witnessing-uncontrolled-forest-fires-1787919-2021-04-06",
            			"image": {
            				"thumbnail": {
            					"contentUrl": "https:\/\/www.bing.com\/th?id=OVFT.H0w-ernjFszXtHcBUUyr3i&pid=News",
            					"width": 647,
            					"height": 363
            				}
            			},
            			"description": "Around 28,000 fires were reported in March. Among the biggest culprits are mahua flower collectors, who burn the forest ground to spot the fallen flowers easily against the scorched earth",
            			"about": [
            				{
            					"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/entities\/cdba16db-da38-4eb1-be8e-ffd949ec3aa9",
            					"name": "Insight"
            				},
            				{
            					"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/entities\/bcbcd891-852b-6dac-1671-8d00b9eae5ea",
            					"name": "Madhya Pradesh"
            				}
            			],
            			"provider": [
            				{
            					"_type": "Organization",
            					"name": "India Today",
            					"image": {
            						"thumbnail": {
            							"contentUrl": "https:\/\/www.bing.com\/th?id=AR_ba698c536218c5b631fb0e182ee8bb41&pid=news"
            						}
            					}
            				}
            			],
            			"datePublished": "2021-04-06T15:01:00.0000000Z"
            		},
            		{
            			"name": "One-horned rhino census stopped temporarily in Nepal as tiger kills mahout",
            			"url": "https:\/\/www.indiablooms.com\/health-details\/W\/9049\/one-horned-rhino-census-stopped-temporarily-in-nepal-as-tiger-kills-mahout.html",
            			"image": {
            				"thumbnail": {
            					"contentUrl": "https:\/\/www.bing.com\/th?id=OVFT.MQGil6cewTDtiSjILNFT9C&pid=News",
            					"width": 700,
            					"height": 462
            				}
            			},
            			"description": "“Once the daily counting of rhinos is completed, elephants are rested. Mahouts usually collect grass during this time or they take their elephants out for grazing,” said Shrestha. “Tharu was attacked while he was cutting grass for his elephant.",
            			"about": [
            				{
            					"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/entities\/bd25e121-3806-b6fe-1af9-3f5745ae2708",
            					"name": "Nepal"
            				},
            				{
            					"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/entities\/767cb442-1c8b-3fc7-a1ea-c91d05e342d5",
            					"name": "Indian rhinoceros"
            				},
            				{
            					"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/entities\/d259ba1c-834d-16c3-8419-6af162776eb7",
            					"name": "Census"
            				}
            			],
            			"provider": [
            				{
            					"_type": "Organization",
            					"name": "India Blooms"
            				}
            			],
            			"datePublished": "2021-04-06T13:21:00.0000000Z"
            		},
            		{
            			"name": "‘Lovely slosh in the pool’: Watch elephants enjoy bath time in mud pool",
            			"url": "https:\/\/indianexpress.com\/article\/trending\/trending-globally\/elephant-africa-muddy-pool-bath-7260964\/",
            			"image": {
            				"thumbnail": {
            					"contentUrl": "https:\/\/www.bing.com\/th?id=OVFT.qXXNq5NA2TJIrQPy2zta1S&pid=News",
            					"width": 700,
            					"height": 389
            				}
            			},
            			"description": "The video, initially shared by Sheldrick Wildlife Trust, Africa’s oldest wildlife charities and conservation organisation on Twitter features their elephants Bondeni, Maisha, Larro and Roho.",
            			"about": [
            				{
            					"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/entities\/34e7b6d7-797b-253e-90ed-3169c8284f4e",
            					"name": "Lovely, Kentucky"
            				},
            				{
            					"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/entities\/a55083a8-0f4c-de67-45ca-de7e28015c11",
            					"name": "The Indian Express"
            				},
            				{
            					"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/entities\/7f12a8c9-f87c-a42f-3bf6-24bee4633b8e",
            					"name": "Backslash"
            				}
            			],
            			"mentions": [
            				{
            					"name": "Lovely, Kentucky"
            				},
            				{
            					"name": "The Indian Express"
            				},
            				{
            					"name": "Backslash"
            				}
            			],
            			"provider": [
            				{
            					"_type": "Organization",
            					"name": "The Indian Express",
            					"image": {
            						"thumbnail": {
            							"contentUrl": "https:\/\/www.bing.com\/th?id=AR_a5cd9487a2ca7779c9fd29b0bff337b0&pid=news"
            						}
            					}
            				}
            			],
            			"datePublished": "2021-04-06T09:53:00.0000000Z"
            		},
            		{
            			"name": "Stray rhinos who entered Assam village, released back in forest",
            			"url": "https:\/\/www.netindia123.com\/netindia\/showdetails.asp?id=3735993&cat=India",
            			"description": "Two one-horned rhinos, who strayed from Orang National Park and Tiger Reserve and entered nearby villages in Assam's Darrang district, were captured by forest officials and returned back to the forest,",
            			"about": [
            				{
            					"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/entities\/a9d4e5df-f559-c28f-dc41-7c72a82dfaf7",
            					"name": "Assam"
            				}
            			],
            			"provider": [
            				{
            					"_type": "Organization",
            					"name": "Net India 123"
            				}
            			],
            			"datePublished": "2021-04-06T09:46:00.0000000Z",
            			"category": "World"
            		},
            		{
            			"name": "Elephant herd goes for a morning walk in delightful viral video. Watch",
            			"url": "https:\/\/www.indiatoday.in\/trending-news\/story\/elephant-herd-goes-for-a-morning-walk-in-delightful-viral-video-watch-1787457-2021-04-05",
            			"description": "Indian Forest Service (IFS) officer Parveen Kaswan shared a video of a herd of elephants taking a walk in the jungle. It seems as if the group of giants is going for a morning walk in the adorable clip. Netizens were stunned by the magnificent sight.",
            			"about": [
            				{
            					"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/entities\/85883125-fe46-e810-e1b9-f83d7d6ffe1b",
            					"name": "Elephant"
            				},
            				{
            					"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/entities\/9b48ec22-0f86-e194-5d34-c861f47c2a1a",
            					"name": "Walk-in"
            				},
            				{
            					"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/entities\/b6651593-cb70-48ef-3f08-baf3e436a8a6",
            					"name": "Herd"
            				}
            			],
            			"provider": [
            				{
            					"_type": "Organization",
            					"name": "India Today"
            				}
            			],
            			"datePublished": "2021-04-05T14:38:00.0000000Z"
            		},
            		{
            			"name": "Video of elephants happily splashing in pool of mud may make you smile",
            			"url": "https:\/\/www.hindustantimes.com\/trending\/video-of-elephants-happily-splashing-in-pool-of-mud-may-make-you-smile-101617616144203.html",
            			"image": {
            				"thumbnail": {
            					"contentUrl": "https:\/\/www.bing.com\/th?id=OVFT.mZhzu0YJJpkql4btMlxQoC&pid=News",
            					"width": 700,
            					"height": 393
            				}
            			},
            			"description": "The clip starts with a shot of the four elephants splashing around a muddy pool. Some videos, featuring playful animals, available on the Internet are an immediate mood-lifter. This video shared by Sheldrick Wildlife Trust on Twitter is a similar and precious addition to that category.",
            			"provider": [
            				{
            					"_type": "Organization",
            					"name": "Hindustan Times",
            					"image": {
            						"thumbnail": {
            							"contentUrl": "https:\/\/www.bing.com\/th?id=AR_121878103001204f6d54fcb8a7699784&pid=news"
            						}
            					}
            				}
            			],
            			"datePublished": "2021-04-05T09:55:00.0000000Z",
            			"category": "LifeStyle"
            		},
            		{
            			"name": "Forty tribal families in Kerala's Ranni don’t know election is on",
            			"url": "https:\/\/www.newindianexpress.com\/states\/kerala\/2021\/apr\/05\/forty-tribal-families-in-keralas-ranni-dont-know-election-is-on-2285866.html",
            			"description": "The biggest is to make sure that their children are not starving. Then there is the fear of elephants that could raid their colony anytime. Living deep inside the forest for weeks together to collect honey, they don’t feel connected to the rest of the world.",
            			"about": [
            				{
            					"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/entities\/9d932c0c-d3e6-abbd-5274-6b53036ca764",
            					"name": "Kerala"
            				}
            			],
            			"provider": [
            				{
            					"_type": "Organization",
            					"name": "The New Indian Express"
            				}
            			],
            			"datePublished": "2021-04-04T22:56:00.0000000Z",
            			"category": "Entertainment"
            		},
            		{
            			"name": "843 elephants died in Odisha in 10 years: Forest and Environment Minister",
            			"url": "https:\/\/orissadiary.com\/843-elephants-died-in-odisha-in-10-years-forest-and-environment-minister\/",
            			"image": {
            				"thumbnail": {
            					"contentUrl": "https:\/\/www.bing.com\/th?id=OVFT.oUS0rju5UMM3fcCQZ9ii-C&pid=News",
            					"width": 700,
            					"height": 526
            				}
            			},
            			"description": "Bhubaneswar: In the 10 years between 2010-11 and 2020-21, human-elephant conflicts caused death of 843 elephants in the State. The highest number of deaths of 93 elephants was reported in year 2018-19 and the lowest 54 was in the year 2014-15. Till March ...",
            			"about": [
            				{
            					"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/entities\/becca699-9820-c027-8e14-b5840348a600",
            					"name": "Odisha"
            				},
            				{
            					"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/entities\/96bb4437-aef6-d4c0-3315-41d9f9b5fb7b",
            					"name": "10 Years"
            				},
            				{
            					"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/entities\/31ab07bd-5f1b-75e0-2d8c-ae6db97c3977",
            					"name": "Environment minister"
            				}
            			],
            			"provider": [
            				{
            					"_type": "Organization",
            					"name": "Odisha Diary"
            				}
            			],
            			"datePublished": "2021-04-04T21:21:00.0000000Z"
            		},
            		{
            			"name": "Tusker Kusha put into Kraal at Dubare Camp",
            			"url": "https:\/\/starofmysore.com\/tusker-kusha-put-into-kraal-at-dubare-camp\/",
            			"image": {
            				"thumbnail": {
            					"contentUrl": "https:\/\/www.bing.com\/th?id=OVFT.eoC9AGqe1Qd1i7f2kKoPAS&pid=News",
            					"width": 700,
            					"height": 325
            				}
            			},
            			"description": "Kraal is a wooden log enclosure into which elephants are driven to be tamed and Dubare Camp has many such Kraals. Kusha, in a ‘Musth’ condition, had ventured into the forests after breaking his heavy chain one-and-a-half years ago in search of a mate ...",
            			"about": [
            				{
            					"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/entities\/c3dde0c2-4e96-4e2f-914d-d02d4b0959d4",
            					"name": "Star of Mysore"
            				},
            				{
            					"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/entities\/964222d5-6172-124e-3423-b176f3cccac6",
            					"name": "Dubare"
            				},
            				{
            					"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/entities\/c2604d41-3df4-50bf-b9d7-d268c3623e9b",
            					"name": "Kraal"
            				}
            			],
            			"provider": [
            				{
            					"_type": "Organization",
            					"name": "Star of Mysore"
            				}
            			],
            			"datePublished": "2021-04-04T13:10:00.0000000Z"
            		},
            		{
            			"name": "Burning Of Mahua Tree Leaves May Have Led To Bandhavgarh Tiger Reserve Fire: Madhya Pradesh Minister",
            			"url": "https:\/\/www.ndtv.com\/india-news\/madhya-pradesh-forest-minister-vijay-shah-burning-of-mahua-tree-leaves-may-have-led-to-bandhavgarh-tiger-reserve-fire-2406005",
            			"description": "Burning of leaves to collect Mahua flowers, lighting a fire to keep wild elephants away or a burning cigarette butt left by someone may have caused the recent blaze in Madhya Pradesh's Bandhavgarh Tiger Reserve, state Forest Minister Vijay Shah said on Sunday.",
            			"provider": [
            				{
            					"_type": "Organization",
            					"name": "NDTV"
            				}
            			],
            			"datePublished": "2021-04-04T13:03:00.0000000Z"
            		},
            		{
            			"name": "Summer Bath Of Elephants In Odisha’s Sambalpur",
            			"url": "https:\/\/odishatv.in\/video\/summer-bath-of-elephants-in-odishas-sambalpur",
            			"image": {
            				"thumbnail": {
            					"contentUrl": "https:\/\/www.bing.com\/th?id=OVFT.jXiMkIXim9f-IQiGwR4NxS&pid=News",
            					"width": 700,
            					"height": 401
            				}
            			},
            			"description": "The locals of Balaranga area in Sambalpur district have captured these scenes of an elephant herd enjoying summer bath at a water body in Sadar forest range.",
            			"about": [
            				{
            					"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/entities\/becca699-9820-c027-8e14-b5840348a600",
            					"name": "Odisha"
            				},
            				{
            					"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/entities\/d544e388-eea9-89a4-5ce9-0083c4c91bc7",
            					"name": "Sambalpur"
            				},
            				{
            					"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/entities\/4c64dfd3-783a-b469-4500-60b947423c38",
            					"name": "Orissa TV"
            				}
            			],
            			"provider": [
            				{
            					"_type": "Organization",
            					"name": "Odisha TV",
            					"image": {
            						"thumbnail": {
            							"contentUrl": "https:\/\/www.bing.com\/th?id=AR_fdb21b190705c05ed35e7bae0b950fc6&pid=news"
            						}
            					}
            				}
            			],
            			"datePublished": "2021-04-04T07:52:00.0000000Z",
            			"video": {
            				"name": "Watch: Summer Bath Of Elephants In Odisha’s Sambalpur - OTV News",
            				"thumbnailUrl": "https:\/\/www.bing.com\/th?id=OVF.X8Ak3tqGZaJKefy%2BxAsz1w&pid=News",
            				"thumbnail": {
            					"width": 300,
            					"height": 172
            				}
            			}
            		},
            		{
            			"name": "Bhopal: Burning of Mahua tree leaves may have led to fire, says Madhya Pradesh minister",
            			"url": "https:\/\/www.freepressjournal.in\/bhopal\/bhopal-burning-of-mahua-tree-leaves-may-have-led-to-fire-says-madhya-pradesh-minister",
            			"description": "Fire in Bandhavgarh Tiger Reserve File Photo Bhopal (Madhya Pradesh): Burning of leaves to collect Mahua flowers, lighting a fire to keep wild elephants away or a burning cigarette butt left by someone may have caused the recent blaze in Madhya Pradesh's ...",
            			"about": [
            				{
            					"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/entities\/7843abc7-b7c1-2b4a-3376-24f0411fbc24",
            					"name": "Bhopal"
            				},
            				{
            					"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/entities\/bcbcd891-852b-6dac-1671-8d00b9eae5ea",
            					"name": "Madhya Pradesh"
            				},
            				{
            					"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/entities\/d18d6402-ddc2-5153-68dc-658367905b0c",
            					"name": "Madhuca longifolia"
            				}
            			],
            			"provider": [
            				{
            					"_type": "Organization",
            					"name": "The Free Press Journal"
            				}
            			],
            			"datePublished": "2021-04-04T07:18:00.0000000Z",
            			"category": "India"
            		},
            		{
            			"name": "Burning Of Mahua Tree Leaves May Have Led To Fire: MP Minister",
            			"url": "https:\/\/www.republicworld.com\/india-news\/general-news\/burning-of-mahua-tree-leaves-may-have-led-to-fire-mp-minister.html",
            			"description": "Burning of leaves to collect Mahua flowers, lighting a fire to keep wild elephants away or a burning cigarette butt left by someone may have caused the recent blaze in Madhya Pradesh's Bandhavgarh Tiger Reserve,",
            			"about": [
            				{
            					"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/entities\/85fa63d3-9596-adb9-b4eb-502273d84f56",
            					"name": "India"
            				}
            			],
            			"provider": [
            				{
            					"_type": "Organization",
            					"name": "RepublicWorld",
            					"image": {
            						"thumbnail": {
            							"contentUrl": "https:\/\/www.bing.com\/th?id=AR_c32961517457f4b62bf48e9fa6fec751&pid=news"
            						}
            					}
            				}
            			],
            			"datePublished": "2021-04-04T07:10:00.0000000Z",
            			"category": "World"
            		},
            		{
            			"name": "Burning of Mahua tree leaves may have led to fire: MP minister By Lemuel Lall",
            			"url": "https:\/\/in.news.yahoo.com\/burning-mahua-tree-leaves-may-074831627.html",
            			"description": "Burning of leaves to collect Mahua flowers, lighting a fire to keep wild elephants away or a burning cigarette butt left by someone may have caused the recent blaze in Madhya Pradesh's Bandhavgarh Tiger Reserve,",
            			"provider": [
            				{
            					"_type": "Organization",
            					"name": "Yahoo News India",
            					"image": {
            						"thumbnail": {
            							"contentUrl": "https:\/\/www.bing.com\/th?id=AR_e1eef5681737e5676d7a09990b13411a&pid=news"
            						}
            					}
            				}
            			],
            			"datePublished": "2021-04-04T00:48:00.0000000Z",
            			"category": "India"
            		},
            		{
            			"name": "Man-elephant conflict claims fifth life in Hassan district",
            			"url": "https:\/\/www.thehindu.com\/news\/national\/karnataka\/man-elephant-conflict-claims-fifth-life-in-hassan-district\/article34232464.ece",
            			"description": "Forest officials handed over a cheque for ₹2 lakh to the family members of the deceased. Around 60 elephants have been roaming around parts of Sakleshpur and Alur taluks of Hassan district. Often, they raid agricultural fields. Forest Minister Arvind ...",
            			"about": [
            				{
            					"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/entities\/96b3e2a7-3ed1-ca93-4ca9-73353c8a6a72",
            					"name": "Hassan district"
            				},
            				{
            					"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/entities\/23f43f24-a2da-029d-b5fe-803cc885d5be",
            					"name": "Conflict"
            				},
            				{
            					"readLink": "https:\/\/api.cognitive.microsoft.com\/api\/v7\/entities\/000e02ee-10d9-417d-a2f9-ec660b7a2930",
            					"name": "Alfil"
            				}
            			],
            			"provider": [
            				{
            					"_type": "Organization",
            					"name": "The Hindu"
            				}
            			],
            			"datePublished": "2021-04-03T15:10:00.0000000Z"
            		},
            		{
            			"name": "Sunday Quiz | On dinosaurs",
            			"url": "https:\/\/www.thehindu.com\/sci-tech\/energy-and-environment\/sunday-quiz-on-dinosaurs\/article34230347.ece",
            			"image": {
            				"thumbnail": {
            					"contentUrl": "https:\/\/www.bing.com\/th?id=OVFT.YJbhqK730kPg1HJ4eqbgnS&pid=News",
            					"width": 615,
            					"height": 384
            				}
            			},
            			"description": "Anonymous",
            			"provider": [
            				{
            					"_type": "Organization",
            					"name": "The Hindu",
        					"image": {
        						"thumbnail": {
        							"contentUrl": "https:\/\/www.bing.com\/th?id=AR_eb38b0dde4b719aca22575dd70e68061&pid=news"
        						}
        					}
        				}
        			],
        			"datePublished": "2021-04-03T10:38:00.0000000Z"
        		}
        	]
        }
    """
